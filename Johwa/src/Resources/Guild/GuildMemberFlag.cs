namespace Johwa.Resources.Guild;

/// <summary>
/// 길드 멤버 플래그 <br/>
/// guild member flags represented as a bit set
/// </summary>
[Flags]
public enum GuildMemberFlag
{
    /// <summary>
    /// 멤버가 길드를 나갔다가 다시 들어왔음 <br/>
    /// Member has left and rejoined the guild
    /// </summary>
    DidRejoin = 1 << 0,

    /// <summary>
    /// 온보딩 완료 <br/>
    /// Member has completed onboarding
    /// </summary>
    CompletedOnboarding = 1 << 1,

    /// <summary>
    /// 인증 요구사항 면제 <br/>
    /// Member is exempt from guild verification requirements
    /// </summary>
    BypassesVerification = 1 << 2,

    /// <summary>
    /// 온보딩 시작 <br/>
    /// Member has started onboarding
    /// </summary>
    StartedOnboarding = 1 << 3,

    /// <summary>
    /// 게스트 멤버 <br/>
    /// Member is a guest and can only access the voice channel they were invited to
    /// </summary>
    IsGuest = 1 << 4,

    /// <summary>
    /// 서버 가이드 신규 멤버 액션 시작 <br/>
    /// Member has started Server Guide new member actions
    /// </summary>
    StartedHomeActions = 1 << 5,

    /// <summary>
    /// 서버 가이드 신규 멤버 액션 완료 <br/>
    /// Member has completed Server Guide new member actions
    /// </summary>
    CompletedHomeActions = 1 << 6,

    /// <summary>
    /// 오토모드에 의해 사용자명, 표시명 또는 닉네임이 차단됨 <br/>
    /// Member's username, display name, or nickname is blocked by AutoMod
    /// </summary>
    AutomodQuarantinedUsername = 1 << 7,

    /// <summary>
    /// DM 설정 업셀을 해제함 <br/>
    /// Member has dismissed the DM settings upsell
    /// </summary>
    DmSettingsUpsellAcknowledged = 1 << 9,
}
