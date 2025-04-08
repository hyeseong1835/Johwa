namespace Johwa.Resources.Guild;

/// <summary>
/// 시스템 채널 플래그 <br/>
/// System Channel Flags
/// </summary>
[Flags]
public enum SystemChannelFlag
{
    /// <summary>
    /// 멤버 가입 알림 숨김 <br/>
    /// Suppress member join notifications
    /// </summary>
    SuppressJoinNotifications = 1 << 0,

    /// <summary>
    /// 서버 부스트 알림 숨김 <br/>
    /// Suppress server boost notifications
    /// </summary>
    SuppressPremiumSubscriptions = 1 << 1,

    /// <summary>
    /// 서버 설정 팁 숨김 <br/>
    /// Suppress server setup tips
    /// </summary>
    SuppressGuildReminderNotifications = 1 << 2,

    /// <summary>
    /// 멤버 가입 스티커 응답 숨김 <br/>
    /// Hide member join sticker reply buttons
    /// </summary>
    SuppressJoinNotificationReplies = 1 << 3,

    /// <summary>
    /// 역할 구독 알림 숨김 <br/>
    /// Suppress role subscription purchase and renewal notifications
    /// </summary>
    SuppressRoleSubscriptionPurchaseNotifications = 1 << 4,

    /// <summary>
    /// 역할 구독 스티커 응답 숨김 <br/>
    /// Hide role subscription sticker reply buttons
    /// </summary>
    SuppressRoleSubscriptionPurchaseNotificationReplies = 1 << 5
}