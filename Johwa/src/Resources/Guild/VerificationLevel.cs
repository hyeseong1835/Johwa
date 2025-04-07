namespace Johwa.Resources.Guild;

/// <summary>
/// 인증 수준 <br/>
/// Verification Level
/// </summary>
public enum VerificationLevel
{
    /// <summary>
    /// 제한 없음 <br/>
    /// unrestricted
    /// </summary>
    None = 0,

    /// <summary>
    /// 이메일 인증 필요 <br/>
    /// must have verified email on account
    /// </summary>
    Low = 1,

    /// <summary>
    /// 가입 후 5분 경과 필요 <br/>
    /// must be registered on Discord for longer than 5 minutes
    /// </summary>
    Medium = 2,

    /// <summary>
    /// 서버 가입 후 10분 경과 필요 <br/>
    /// must be a member of the server for longer than 10 minutes
    /// </summary>
    High = 3,

    /// <summary>
    /// 전화번호 인증 필요 <br/>
    /// must have a verified phone number
    /// </summary>
    VeryHigh = 4
}
