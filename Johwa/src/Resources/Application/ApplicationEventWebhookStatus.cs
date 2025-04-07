namespace Johwa.Resources.Application;

/// <summary>
/// 애플리케이션의 웹훅 이벤트 사용 상태 <br/>
/// Status indicating whether event webhooks are enabled or disabled for an application.
/// </summary>
public enum ApplicationEventWebhookStatus
{
    /// <summary>
    /// 개발자가 웹훅 이벤트를 비활성화함 <br/>
    /// Webhook events are disabled by developer
    /// </summary>
    Disabled = 1,

    /// <summary>
    /// 개발자가 웹훅 이벤트를 활성화함 <br/>
    /// Webhook events are enabled by developer
    /// </summary>
    Enabled = 2,

    /// <summary>
    /// Discord가 웹훅 이벤트를 비활성화함 <br/>
    /// Webhook events are disabled by Discord, usually due to inactivity
    /// </summary>
    DisabledByDiscord = 3
}
