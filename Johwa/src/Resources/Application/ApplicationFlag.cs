namespace Johwa.Resources.Application;

/// <summary>
/// 애플리케이션의 플래그 정보 <br/>
/// Flags representing properties of the application
/// </summary>
[Flags]
public enum ApplicationFlag
{
    /// <summary>
    /// 앱이 자동 모더레이션 API를 사용하는 경우 <br/>
    /// Indicates if an app uses the Auto Moderation API
    /// </summary>
    AutoModerationRuleCreateBadge = 1 << 6,

    /// <summary>
    /// 100개 이상의 서버에서 presence_update 이벤트를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in 100 or more servers to receive presence_update events
    /// </summary>
    GatewayPresence = 1 << 12,

    /// <summary>
    /// 100개 미만의 서버에서 presence_update 이벤트를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in under 100 servers to receive presence_update events
    /// </summary>
    GatewayPresenceLimited = 1 << 13,

    /// <summary>
    /// 100개 이상의 서버에서 member 관련 이벤트를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in 100 or more servers to receive member-related events
    /// </summary>
    GatewayGuildMembers = 1 << 14,

    /// <summary>
    /// 100개 미만의 서버에서 member 관련 이벤트를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in under 100 servers to receive member-related events
    /// </summary>
    GatewayGuildMembersLimited = 1 << 15,

    /// <summary>
    /// 앱의 비정상적인 성장으로 인해 인증이 제한됨 <br/>
    /// Indicates unusual growth of an app that prevents verification
    /// </summary>
    VerificationPendingGuildLimit = 1 << 16,

    /// <summary>
    /// 앱이 Discord 클라이언트 내에 내장되어 있음 (공개되지 않음) <br/>
    /// Indicates if an app is embedded within the Discord client (currently unavailable publicly)
    /// </summary>
    Embedded = 1 << 17,

    /// <summary>
    /// 100개 이상의 서버에서 메시지 콘텐츠를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in 100 or more servers to receive message content
    /// </summary>
    GatewayMessageContent = 1 << 18,

    /// <summary>
    /// 100개 미만의 서버에서 메시지 콘텐츠를 수신하려면 필요한 intent <br/>
    /// Intent required for bots in under 100 servers to receive message content
    /// </summary>
    GatewayMessageContentLimited = 1 << 19,

    /// <summary>
    /// 앱이 글로벌 애플리케이션 커맨드를 등록한 경우 <br/>
    /// Indicates if an app has registered global application commands
    /// </summary>
    ApplicationCommandBadge = 1 << 23
}
