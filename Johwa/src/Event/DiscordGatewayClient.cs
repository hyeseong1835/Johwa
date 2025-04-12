using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Johwa.Event;

public class DiscordGatewayClient
{
    # region 정적

    const string GatewayUrl = "wss://gateway.discord.gg/?v=10&encoding=json";
    static readonly Uri gatewayUri = new Uri(GatewayUrl);

    // 디스패치 이벤트
    public readonly DispatchEventGroupDictionary dispatchEventDictionary = new();

    #endregion

    #region 필드
    
    public string token;
    public GatewayIdentifyProperties gatewayIdentifyProperties;
    public int receiveBufferSize;

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

    public DiscordGatewayClient(string token, GatewayIdentifyProperties? gatewayIdentifyProperties = null, int receiveBufferSize = 8192)
    {
        this.receiveBufferSize = receiveBufferSize;
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
        
        //string prettyJsonString = JsonUtility.ToPrettyJsonString(jsonString);
        //Console.WriteLine($"[ 로그 ] 송신 ({byteCount}bytes): \n{prettyJsonString}");
    }

    void StartReceiveLoop()
    {
        if (receiveLoopTask != null) {
            Console.WriteLine("[ 오류 ] 수신 루프가 이미 실행 중입니다.");
            return;
        }
        if (receiveLoopCts != null) {
            Console.WriteLine("[ 오류 ] 수신 루프 취소 토큰이 정리되지 않았습니다.");
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

        HandleEvent(buffer);
    }

    #region 이벤트 핸들링 메서드

    void ReadGatewayPayload(ReadOnlySpan<byte> span, 
        out GatewayOpcode op, out ReadOnlySpan<byte> d, out int s, out ReadOnlySpan<byte> t)
    {
        Utf8JsonReader reader = new(span);

        op = default;
        d = default;
        s = default;
        t = default;

        bool isOpFound = false;
        bool isDFound = false;
        bool isSFound = false;
        bool isTFound = false;

        while (reader.Read())
        {
            // 깊이 1만 탐색
            if (reader.CurrentDepth != 1)
                continue;
            
            // 프로퍼티 이름만 탐색
            if (reader.TokenType != JsonTokenType.PropertyName) 
                continue;

            // Opcode
            if (isOpFound == false && reader.ValueTextEquals("op"))
            {
                // 값으로 이동
                reader.Read();

                op = (GatewayOpcode)reader.GetInt32();
                isOpFound = true;

                if (isDFound && isSFound && isTFound)
                    return;
            }
            
            // Data
            if (isDFound == false && reader.ValueTextEquals("d"))
            {
                // 값으로 이동
                reader.Read();

                d = reader.ValueSpan;

                if (isOpFound && isSFound && isTFound)
                    return;

                isDFound = true;
            }

            // "d" 키를 찾으면 오브젝트 내용을 읽어 저장하는 코드
            if (!isDFound && reader.ValueTextEquals("d"))
            {
                reader.Read(); // "d"의 값으로 이동 (StartObject)

                if (reader.TokenType != JsonTokenType.StartObject){
                    throw new JsonException("Expected StartObject token for 'd' property.");
                }

                // 객체 시작 위치 기록
                int start = (int)reader.TokenStartIndex; 

                int depth = reader.CurrentDepth;

                // 객체가 끝날 때까지 반복하여 읽기
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth - 1)
                    {
                        // 객체의 끝 위치
                        int end = (int)reader.BytesConsumed; 

                        // 원본 JSON에서 객체에 해당하는 부분을 추출
                        d = span.Slice(start, end - start);
                        break;
                    }
                }
                if (d == default) {
                    throw new JsonException("Failed to read 'd' property as object.");
                }
            }
            
            // Sequence
            if (isSFound == false && reader.ValueTextEquals("s"))
            {
                // 값으로 이동
                reader.Read();

                if (reader.TokenType == JsonTokenType.Null)
                {
                    t = default;
                    isTFound = true;

                    s = default;

                    if (isOpFound && isDFound) 
                        return;
                    
                    isSFound = true;
                    continue;
                }

                s = reader.GetInt32();
                isSFound = true;

                if (isOpFound && isDFound && isTFound)
                    return;
            }
            
            // Type
            if (isTFound == false && reader.ValueTextEquals("t"))
            {
                // 값으로 이동
                reader.Read();

                if (reader.TokenType == JsonTokenType.Null)
                {
                    s = default;
                    isSFound = true;

                    t = default;
                    isTFound = true;

                    if (isOpFound && isDFound) 
                        return;
                    
                    continue;
                }

                t = reader.ValueSpan;

                if (isOpFound && isDFound && isSFound)
                    return;

                isTFound = true;
            }
        }
    }
    
    void HandleEvent(byte[] buffer)
    {
        // 수신된 데이터 처리
        GatewayOpcode operationCode;    // operation code
        ReadOnlySpan<byte> data; // 데이터
        int sequence;            // 시퀀스 번호 (Dispatch만)
        ReadOnlySpan<byte> type; // 이벤트 타입 (Dispatch만)

        ReadGatewayPayload(buffer, out operationCode, out data, out sequence, out type);

        // operationCode 따라서 처리
        switch (operationCode)
        {
            // 0
            case GatewayOpcode.Dispatch: 
            {
                _lastSequence = sequence;

                // 등록되지 않은 이벤트는 건너뛰기
                if (dispatchEventDictionary.TryGetValue(type, out DispatchEventGroup? eventGroup) == false) {
                    break;
                }

                eventGroup.OnHandled(this, data);

                break; 
            }
            // 7
            case GatewayOpcode.Reconnect: {
                Task.Run(Reconnect);
                break;
            }
            // 8
            case GatewayOpcode.InvalidSession: 
            {
                Utf8JsonReader reader = new Utf8JsonReader(data);
                reader.Read();
                if (reader.TokenType == JsonTokenType.True)
                {
                    Task.Run(Reconnect);
                }
                else
                {

                }
                break;
            }
            // 10
            case GatewayOpcode.Hello: 
            {
                int heartbeatInterval = -1;
                Utf8JsonReader reader = new Utf8JsonReader(data);
                while (reader.Read())
                {
                    if (reader.CurrentDepth != 1) continue;

                    if (reader.TokenType != JsonTokenType.PropertyName) continue;
                    if (reader.ValueTextEquals("heartbeat_interval"))
                    {
                        reader.Read();
                        heartbeatInterval = reader.GetInt32();
                        break;
                    }
                }
                if (heartbeatInterval == -1) {
                    Console.WriteLine("[ 오류 ] Heartbeat Interval을 찾을 수 없습니다.");
                    return;
                }

                // 하트비트 루프 시작
                StartHeartbeatLoop(heartbeatInterval); 

                // Identify 페이로드 전송
                _ = Task.Run(SendIdentifyPayLoad);

                break;
            }
            // 11
            case GatewayOpcode.HeartbeatAck: {
                _lastHeartbeatAck = DateTime.Now;
                break;
            }

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

    #endregion

    #endregion


    #region 도구

    #region DispatchEvent

    #endregion

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
