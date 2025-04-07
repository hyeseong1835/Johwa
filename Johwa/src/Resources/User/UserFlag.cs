
namespace Johwa.Resources.User;

/// <summary>
/// 사용자의 특수한 상태나 역할을 나타내는 플래그입니다. <br/>
/// Flags representing special statuses or roles assigned to a user.
/// </summary>
[Flags]
public enum UserFlag
{
    /// <summary>
    /// 디스코드 직원 <br/>
    /// Discord Employee
    /// </summary>
    STAFF = 1 << 0,

    /// <summary>
    /// 파트너 서버 소유자 <br/>
    /// Partnered Server Owner
    /// </summary>
    PARTNER = 1 << 1,

    /// <summary>
    /// 하이프스쿼드 이벤트 멤버 <br/>
    /// HypeSquad Events Member
    /// </summary>
    HYPESQUAD = 1 << 2,

    /// <summary>
    /// 버그 헌터 레벨 1 <br/>
    /// Bug Hunter Level 1
    /// </summary>
    BUG_HUNTER_LEVEL_1 = 1 << 3,

    /// <summary>
    /// 하우스 브레이버리 멤버 <br/>
    /// House Bravery Member
    /// </summary>
    HYPESQUAD_ONLINE_HOUSE_1 = 1 << 6,

    /// <summary>
    /// 하우스 브릴리언스 멤버 <br/>
    /// House Brilliance Member
    /// </summary>
    HYPESQUAD_ONLINE_HOUSE_2 = 1 << 7,

    /// <summary>
    /// 하우스 밸런스 멤버 <br/>
    /// House Balance Member
    /// </summary>
    HYPESQUAD_ONLINE_HOUSE_3 = 1 << 8,

    /// <summary>
    /// 초기 Nitro 후원자 <br/>
    /// Early Nitro Supporter
    /// </summary>
    PREMIUM_EARLY_SUPPORTER = 1 << 9,

    /// <summary>
    /// 팀 사용자 <br/>
    /// User is a team
    /// </summary>
    TEAM_PSEUDO_USER = 1 << 10,

    /// <summary>
    /// 버그 헌터 레벨 2 <br/>
    /// Bug Hunter Level 2
    /// </summary>
    BUG_HUNTER_LEVEL_2 = 1 << 14,

    /// <summary>
    /// 인증된 봇 <br/>
    /// Verified Bot
    /// </summary>
    VERIFIED_BOT = 1 << 16,

    /// <summary>
    /// 초기 인증 봇 개발자 <br/>
    /// Early Verified Bot Developer
    /// </summary>
    VERIFIED_DEVELOPER = 1 << 17,

    /// <summary>
    /// 인증된 모더레이터 프로그램 졸업생 <br/>
    /// Moderator Programs Alumni
    /// </summary>
    CERTIFIED_MODERATOR = 1 << 18,

    /// <summary>
    /// 봇이 HTTP 상호작용만 사용하며 온라인 목록에 표시됨 <br/>
    /// Bot uses only HTTP interactions and is shown in the online member list
    /// </summary>
    BOT_HTTP_INTERACTIONS = 1 << 19,

    /// <summary>
    /// 활성 개발자 <br/>
    /// User is an Active Developer
    /// </summary>
    ACTIVE_DEVELOPER = 1 << 22,
}
