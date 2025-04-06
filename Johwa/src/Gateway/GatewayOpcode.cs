
namespace Johwa.Gateway;

/// <summary>
/// Discord Gateway에서 사용하는 Opcode 열거형입니다.<br/>
/// 각 Opcode는 클라이언트와 Discord 서버 간의 통신에서 어떤 동작을 수행하는지 나타냅니다.
/// </summary>
public enum GatewayOpcode : int
{
    /// <summary> 이벤트가 디스패치됨. (수신) </summary>
    Dispatch = 0,

    /// <summary> 클라이언트가 주기적으로 연결을 유지하기 위해 전송함. (송신/수신) </summary>
    Heartbeat = 1,

    /// <summary> 초기 핸드셰이크 동안 새로운 세션을 시작함. (송신) </summary>
    Identify = 2,

    /// <summary> 클라이언트의 현재 상태(presence)를 업데이트함. (송신) </summary>
    PresenceUpdate = 3,

    /// <summary> 음성 채널에 참가/이탈하거나 이동할 때 사용됨. (송신) </summary>
    VoiceStateUpdate = 4,

    /// <summary> 끊긴 이전 세션을 재개함. (송신) </summary>
    Resume = 6,

    /// <summary> 즉시 재접속하고 세션을 재개해야 함. (수신) </summary>
    Reconnect = 7,

    /// <summary> 대형 길드에서 오프라인 구성원 정보를 요청함. (송신) </summary>
    RequestGuildMembers = 8,

    /// <summary> 세션이 무효화됨. 재접속 후 Identify 또는 Resume을 수행해야 함. (수신) </summary>
    InvalidSession = 9,

    /// <summary> 연결 직후 전송되며, 사용할 heartbeat 간격 정보를 포함함. (수신) </summary>
    Hello = 10,

    /// <summary> Heartbeat 수신을 확인하는 응답으로 전송됨. (수신) </summary>
    HeartbeatAck = 11,

    /// <summary> 특정 길드들에 대한 사운드보드 사운드 정보를 요청함. (송신) </summary>
    RequestSoundboardSounds = 31
}
