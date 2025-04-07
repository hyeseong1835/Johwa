namespace Johwa.Resources.Guild;

/// <summary>
/// 기본 메시지 알림 수준 <br/>
/// Default message notifications level
/// </summary>
public enum DefaultMessageNotificationLevel
{
    /// <summary>
    /// 모든 메시지에 대해 알림을 받음 <br/>
    /// members will receive notifications for all messages by default
    /// </summary>
    AllMessages = 0,

    /// <summary>
    /// 멘션된 메시지만 알림 <br/>
    /// members will receive notifications only for messages that @mention them by default
    /// </summary>
    OnlyMentions = 1
}