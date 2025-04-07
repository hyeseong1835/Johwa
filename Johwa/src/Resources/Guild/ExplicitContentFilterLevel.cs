namespace Johwa.Resources.Guild;

/// <summary>
/// 노골적인 콘텐츠 필터 수준 <br/>
/// Explicit content filter level
/// </summary>
public enum ExplicitContentFilterLevel
{
    /// <summary>
    /// 콘텐츠 스캔 안 함 <br/>
    /// media content will not be scanned
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 역할이 없는 멤버의 콘텐츠만 스캔 <br/>
    /// media content sent by members without roles will be scanned
    /// </summary>
    MembersWithoutRoles = 1,

    /// <summary>
    /// 모든 멤버의 콘텐츠 스캔 <br/>
    /// media content sent by all members will be scanned
    /// </summary>
    AllMembers = 2
}