using System.Buffers;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Johwa.Core;
using Johwa.Utility.Json;

namespace Johwa.Event;

public class DiscordGatewayClient
{
    # region 정적

    const string GatewayUrl = "wss://gateway.discord.gg/?v=10&encoding=json";
    static readonly Uri gatewayUri = new Uri(GatewayUrl);

    // 디스패치 이벤트
    public static readonly Dictionary<string, int> dispatchEventIndexDictionary = new();
    static List<DispatchEventGroup> dispatchEventGroupList = new();

    #endregion

    #region 필드
    
    public string token;
    public GatewayIdentifyProperties gatewayIdentifyProperties;

    // 웹소켓 연결
    readonly ClientWebSocket webSocket;
    CancellationTokenSource? webSocketCts;

    // 수신 루프
    Task? receiveLoopTask;
    CancellationTokenSource? receiveLoopCts;

    int? _lastSequence = null;
    
    #region Heartbeat

    Task? heartbeatTask;
    CancellationTokenSource? heartbeatCts;
    int curHeartbeatInterval;
    DateTime _lastHeartbeatSent;
    DateTime _lastHeartbeatAck;

    #endregion

    #endregion

    public DiscordGatewayClient(string token, GatewayIdentifyProperties? gatewayIdentifyProperties = null)
    {
        this.token = token;
        this.gatewayIdentifyProperties = gatewayIdentifyProperties?? new GatewayIdentifyProperties();

        this.webSocket = new ClientWebSocket();
    }


    #region 연결
    
    public async Task Connect()
    {
        if (webSocket.State == WebSocketState.Open) {
            Console.WriteLine("[ 오류 ] 이미 웹소켓이 연결되어 있습니다.");
            return;
        }

        // 취소 토큰 생성
        if (webSocketCts != null) {
            Console.WriteLine("[ 오류 ] 웹소켓 취소 토큰이 정리되지 않았습니다.");
            return;
        }
        webSocketCts = new CancellationTokenSource();

        // 웹소켓 연결
        await webSocket.ConnectAsync(gatewayUri, webSocketCts.Token);

        // 수신 루프 시작
        StartReceiveLoop();

        Console.WriteLine("[ 로그 ] 웹소켓 연결됨");
    }
    
    public async Task Disconnect()
    {
        // 웹소켓 연결 중단
        await StopWebSocket();

        // 웹소켓 연결 중단 토큰 정리
        ClearWebSocketCts();

        // 하트비트 루프 중단
        StopHeartbeatLoop();
        
        Console.WriteLine("[ 로그 ] 웹소켓 연결 해제됨");
    }

    public async Task Reconnect()
    {
        await Disconnect();
        await Connect();
    }

    #endregion


    #region 기본

    async Task SendAsync(string json)
    {
        // 문자열의 UTF8 인코딩 최대 길이 계산 (UTF8은 문자당 최대 4바이트)
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(json.Length);

        // 버퍼 대여
        byte[] buffer = ArrayPool<byte>.Shared.Rent(maxByteCount);

        try
        {
            int byteCount = Encoding.UTF8.GetBytes(json, 0, json.Length, buffer, 0);

            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        finally
        {
            // 버퍼 반환
            ArrayPool<byte>.Shared.Return(buffer);
        }


        using JsonDocument doc = JsonDocument.Parse(json);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string prettyJson = JsonSerializer.Serialize(doc.RootElement, options);
        Console.WriteLine($"[ 로그 ] 송신 ({buffer.Length}bytes): \n{json}");
    }
    async Task SendAsync(object jsonObj)
    {
        string jsonString = JsonSerializer.Serialize(jsonObj);
        
        // 문자열의 UTF8 인코딩 최대 길이 계산 (UTF8은 문자당 최대 4바이트)
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(jsonString.Length);

        // 버퍼 대여
        int byteCount = 0;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(maxByteCount);
        try
        {
            byteCount = Encoding.UTF8.GetBytes(jsonString, 0, jsonString.Length, buffer, 0);

            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, byteCount), 
                WebSocketMessageType.Text, 
                true, 
                CancellationToken.None
            );
        }
        finally
        {
            // 버퍼 반환
            ArrayPool<byte>.Shared.Return(buffer);
        }
        
        string prettyJsonString = JsonUtility.ToPrettyJsonString(jsonString);
        Console.WriteLine($"[ 로그 ] 송신 ({byteCount}bytes): \n{prettyJsonString}");
    }

    void StartReceiveLoop()
    {
        if (receiveLoopTask != null) {
            Console.WriteLine("[ 오류 ] 수신 루프가 이미 실행 중입니다.");
            return;
        }
        receiveLoopCts = new CancellationTokenSource();

        // 수신 루프 시작
        receiveLoopTask = Task.Run(async () => {
            // 버퍼 대여
            byte[] buffer = ArrayPool<byte>.Shared.Rent(8192);
            try
            {
                while (webSocket.State == WebSocketState.Open && webSocketCts != null && !webSocketCts.Token.IsCancellationRequested)
                {
                    await ReceiveEvent(buffer, receiveLoopCts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ 오류 ] 이벤트 수신 오류: \n{ex.Message}");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        });
    }
    async Task StopReceiveLoop()
    {
        await CancelReceiveLoop();
    }
    async Task CancelReceiveLoop()
    {
        if (receiveLoopCts == null) return;
        
        try
        {
            receiveLoopCts.Cancel();
            if (receiveLoopTask != null) {
                await receiveLoopTask;
                receiveLoopTask = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ 오류 ] 수신 루프 취소 중 오류: \n{ex.Message}");
        }
        finally
        {
            receiveLoopCts.Dispose();
            receiveLoopCts = null;
        }
    }
    async Task ReceiveEvent(byte[] buffer, CancellationToken cancellationToken)
    {
        // 수신 대기
        WebSocketReceiveResult result;
        try
        {
            result = await webSocket.ReceiveAsync(buffer, cancellationToken);
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"[ 오류 ] 웹소켓 수신 오류: \n{ex.Message}");
            return;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[ 로그 ] 수신 루프 취소");
            await CancelReceiveLoop();
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ 오류 ] 수신 오류: \n{ex.Message}");
            return;
        }

        if (result == null) return;

        if (result.MessageType == WebSocketMessageType.Close) {
            Console.WriteLine($"[ 로그 ] 수신: 웹소켓 연결 종료 ({result.CloseStatus})");
            return;
        }

        string jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);
        await HandleEvent(jsonString);
    }

    #region 이벤트 핸들링 메서드

    async Task HandleEvent(string jsonString)
    {
        // 수신된 데이터 처리
        string prettyJsonString = JsonUtility.ToPrettyJsonString(jsonString);
        JsonElement payload = JsonSerializer.Deserialize<JsonElement>(jsonString);

        // opcode 따라서 처리
        GatewayOpcode operationCode = (GatewayOpcode)payload.GetProperty("op").GetInt32();
        Console.WriteLine($"[ 로그 ] 수신: {operationCode} \n{prettyJsonString}");

        switch (operationCode)
        {
            // 0
            case GatewayOpcode.Dispatch: await HandleDispatch(payload); break; 
            // 7
            case GatewayOpcode.Reconnect: await HandleReconnect(payload); break;
            // 8
            case GatewayOpcode.InvalidSession: await HandleInvalidSession(payload); break;
            // 10
            case GatewayOpcode.Hello: await HandleHello(payload); break;
            // 11
            case GatewayOpcode.HeartbeatAck: await HandleHeartbeatAck(payload); break;

            // 클라이언트에서 전송하는 페이로드
            case GatewayOpcode.Heartbeat: //1
            case GatewayOpcode.Identify: //2
            case GatewayOpcode.PresenceUpdate: //3
            case GatewayOpcode.VoiceStateUpdate: //4
            case GatewayOpcode.Resume: //6
            case GatewayOpcode.RequestGuildMembers: //8
            case GatewayOpcode.RequestSoundboardSounds: //31
                break;

            default:
                Console.WriteLine($"[ 오류 ] 대응하지 않은 opcode: {operationCode}");
                break;
        }
    }

    async Task HandleDispatch(JsonElement payload)
    {
        // 시퀀스 번호 추출
        JsonElement sequenceProperty;
        if (payload.TryGetProperty("s", out sequenceProperty) 
            && sequenceProperty.ValueKind != JsonValueKind.Null)
        {
            _lastSequence = sequenceProperty.GetInt32();
        }

        JsonElement eventTypeProperty;
        if (payload.TryGetProperty("t", out eventTypeProperty) 
            && eventTypeProperty.ValueKind == JsonValueKind.String)
        {
            string? eventType = eventTypeProperty.GetString();
            if (eventType == null) {
                Console.WriteLine("Event type is null");
                return;
            }

            int eventIndex;
            DispatchEventGroup dispatchEventGroup;
            if (dispatchEventIndexDictionary.TryGetValue(eventType, out eventIndex)) 
            {
                dispatchEventGroup = dispatchEventGroupList[eventIndex];
            }
            else
            {
                dispatchEventGroup = new DispatchEventGroup();
                dispatchEventIndexDictionary.Add(eventType, dispatchEventGroupList.Count);
                dispatchEventGroupList.Add(dispatchEventGroup);
            }
            JsonElement data = payload.GetProperty("d");
            await dispatchEventGroup.Execute(this, data);
        }
    }
    async Task HandleReconnect(JsonElement payload)
    {
        await Reconnect();
    }
    async Task HandleInvalidSession(JsonElement payload)
    {
        // 세션이 무효화됨 → Identify 또는 Resume 수행
        if (payload.FindBoolean("d"))
        {
            await Disconnect();

            await SendIdentifyPayLoad();
        }
        else
        {

        }
    }
    async Task HandleHello(JsonElement payload)
    {
        JsonElement dataProperty = payload.GetProperty("d");
        int interval = dataProperty.GetProperty("heartbeat_interval").GetInt32();

        // Identify는 Hello 이후에 반드시 보내야 함
        
        _ = Task.Run(async () => {
            await Task.Delay(100); 
            await SendIdentifyPayLoad();
        });

        StartHeartbeatLoop(interval);
    }

    async Task HandleHeartbeatAck(JsonElement payload)
    {
        _lastHeartbeatAck = DateTime.Now;
    }

    #endregion

    #endregion


    #region 도구

    async Task StopWebSocket()
    {
        if (webSocket == null) return;
        
        try
        {
            if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ 오류 ] 웹 소켓 연결 해제 중 오류: \n{ex.Message}");
        }
        finally
        {
            webSocket.Dispose();
        }
    }
    void ClearWebSocketCts()
    {
        if (webSocketCts == null) return;
        
        try
        {
            webSocketCts.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ 오류 ] 웹 소켓 연결 해제 중 오류: \n{ex.Message}");
        }
        finally
        {
            webSocketCts.Dispose();
            webSocketCts = null;
        }
    }
    
    #region 하트비트
    
    void StopHeartbeatLoop()
    {
        if (heartbeatCts == null) return;
        
        try
        {
            heartbeatCts.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ 오류 ] 하트비트 루프 중단 중 오류: \n{ex.Message}");
        }
        finally
        {
            heartbeatCts.Dispose();
            heartbeatCts = null;
            heartbeatTask = null;
            curHeartbeatInterval = 0;
        }
    }
    
    void StartHeartbeatLoop(int intervalMs)
    {
        // 현재 루프가 살아 있고 동일한 간격이면 새로 만들지 않음
        if (heartbeatTask != null
            && (heartbeatTask.IsCompleted == false && curHeartbeatInterval == intervalMs)) {
            Console.WriteLine("[ 오류 ] 이미 Heartbeat 루프가 실행 중입니다.");
            return;
        }

        // 기존 루프 취소
        heartbeatCts?.Cancel();
        heartbeatCts = new CancellationTokenSource();
        curHeartbeatInterval = intervalMs;

        // 새로운 루프 시작
        heartbeatTask = Task.Run(() => HeartbeatLoop(intervalMs, heartbeatCts.Token));
    }
    private async Task HeartbeatLoop(int intervalMs, CancellationToken cancelToken)
    {
        try
        {
            while (true)
            {
                if (webSocket.State != WebSocketState.Open) {
                    Console.WriteLine("[ 로그 ] 하트비트 루프 중단: 웹소켓이 닫혔습니다.");
                    break;
                }
                if (cancelToken.IsCancellationRequested) {
                    Console.WriteLine("[ 로그 ] 하트비트 루프 중단: 취소 요청됨.");
                    break;
                }
                var heartbeatPayload = new
                {
                    op = GatewayOpcode.Heartbeat,
                    d = _lastSequence
                };

                await SendAsync(heartbeatPayload);

                _lastHeartbeatSent = DateTime.Now;

                await Task.Delay(intervalMs, cancelToken);
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Heartbeat 중단됨");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Heartbeat 오류: {ex.Message}");
        }
    }

    #endregion
    
    /// <summary> Identify payload 전송 </summary>
    public async Task SendIdentifyPayLoad()
    {
        var identifyPayload = new
        {
            op = GatewayOpcode.Identify,
            d = new
            {
                token = token,
                intents = 513,
                properties = new
                {
                    os = gatewayIdentifyProperties.os,
                    browser = gatewayIdentifyProperties.browser,
                    device = gatewayIdentifyProperties.device
                }
            }
        };

        await SendAsync(identifyPayload);
    }
    
    #endregion
}
