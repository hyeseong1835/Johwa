namespace Johwa.Resources.Guild;

/// <summary>
/// 서버 부스트 티어 <br/>
/// Premium Tier (Server Boost Level)
/// </summary>
public enum PremiumTier
{
    /// <summary>
    /// 부스트 없음 <br/>
    /// guild has not unlocked any Server Boost perks
    /// </summary>
    None = 0,

    /// <summary>
    /// 티어 1 <br/>
    /// guild has unlocked Server Boost level 1 perks
    /// </summary>
    Tier1 = 1,

    /// <summary>
    /// 티어 2 <br/>
    /// guild has unlocked Server Boost level 2 perks
    /// </summary>
    Tier2 = 2,

    /// <summary>
    /// 티어 3 <br/>
    /// guild has unlocked Server Boost level 3 perks
    /// </summary>
    Tier3 = 3
}