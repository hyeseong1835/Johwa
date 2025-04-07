namespace Johwa.Resources.Guild;

/// <summary>
/// NSFW 수준 <br/>
/// Guild NSFW Level
/// </summary>
public enum NsfwLevel
{
    /// <summary>
    /// 기본 <br/>
    /// DEFAULT
    /// </summary>
    Default = 0,

    /// <summary>
    /// 노출성 콘텐츠 포함 <br/>
    /// EXPLICIT
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// 안전한 콘텐츠만 포함 <br/>
    /// SAFE
    /// </summary>
    Safe = 2,

    /// <summary>
    /// 연령 제한 콘텐츠 포함 <br/>
    /// AGE_RESTRICTED
    /// </summary>
    AgeRestricted = 3
}