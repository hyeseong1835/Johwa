
namespace Johwa.Gateway;

/// <summary>
/// Discord Gateway Close 이벤트 코드 열거형입니다.<br/>
/// 연결 종료 시 서버에서 전달되며, 일부 코드는 재연결이 허용되지 않습니다.
/// </summary>
public enum GatewayCloseEventCodes : int
{
    /// <summary> 알 수 없는 오류. (재연결 가능) </summary>
    UnknownError = 4000,

    /// <summary> 잘못된 opcode 또는 payload 전송. (재연결 가능) </summary>
    UnknownOpcode = 4001,

    /// <summary> 페이로드 디코딩 실패. (재연결 가능) </summary>
    DecodeError = 4002,

    /// <summary> 인증되지 않은 상태에서 페이로드 전송. (재연결 가능) </summary>
    NotAuthenticated = 4003,

    /// <summary> 잘못된 계정 토큰으로 인증 실패. (재연결 불가) </summary>
    AuthenticationFailed = 4004,

    /// <summary> 이미 인증된 상태에서 중복 식별 시도. (재연결 가능) </summary>
    AlreadyAuthenticated = 4005,

    /// <summary> 재개 시 잘못된 시퀀스 번호 사용. (재연결 가능) </summary>
    InvalidSeq = 4007,

    /// <summary> 너무 빠르게 페이로드 전송함. (재연결 가능) </summary>
    RateLimited = 4008,

    /// <summary> 세션 유효 시간이 초과됨. (재연결 가능) </summary>
    SessionTimedOut = 4009,

    /// <summary> 잘못된 샤드 ID 사용. (재연결 불가) </summary>
    InvalidShard = 4010,

    /// <summary> 샤딩이 필요함. (재연결 불가) </summary>
    ShardingRequired = 4011,

    /// <summary> 지원되지 않는 API 버전 사용. (재연결 불가) </summary>
    InvalidApiVersion = 4012,

    /// <summary> 잘못된 Gateway Intent 설정. (재연결 불가) </summary>
    InvalidIntents = 4013,

    /// <summary> 허용되지 않은 Gateway Intent 사용. (재연결 불가) </summary>
    DisallowedIntents = 4014
}
