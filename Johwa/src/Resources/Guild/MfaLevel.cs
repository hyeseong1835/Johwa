namespace Johwa.Resources.Guild;

/// <summary>
/// MFA 수준 <br/>
/// MFA Level
/// </summary>
public enum MfaLevel
{
    /// <summary>
    /// MFA 없음 <br/>
    /// guild has no MFA/2FA requirement for moderation actions
    /// </summary>
    None = 0,

    /// <summary>
    /// 2FA 필수 <br/>
    /// guild has a 2FA requirement for moderation actions
    /// </summary>
    Elevated = 1
}