using System.Text.Json;
using Johwa.Common;
using Johwa.Common.Json;
using Johwa.Utility;
using Johwa.Utility.Json;
using Johwa.Utility.StringResourceArrayUtility;

namespace Johwa.Resources.Guild;

public struct GuildObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public GuildObject(JsonElement guildProperty)
    {
        Property = guildProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 길드 ID <br/>
    /// guild id
    /// </summary>
    public Snowflake Id 
        => Property.FindSnowflake("id");

    /// <summary>
    /// [ name ] <br/>
    /// 길드 이름 <br/>
    /// guild name (2-100 characters, excluding trailing and leading whitespace)
    /// </summary>
    public string Name 
        => Property.FindString("name");

    /// <summary>
    /// [ icon ] <br/>
    /// 아이콘 해시 <br/>
    /// icon hash
    /// </summary>
    public string? Icon 
        => Property.FindStringOrNull("icon");

    /// <summary>
    /// [ splash ] <br/>
    /// 스플래시 해시 <br/>
    /// splash hash
    /// </summary>
    public string? Splash 
        => Property.FindStringOrNull("splash");
    
    /// <summary>
    /// [ discovery_splash ] <br/>
    /// 디스커버리 스플래시 해시 <br/>
    /// discovery splash hash
    /// </summary>
    public string? DiscoverySplash 
        => Property.FindStringOrNull("discovery_splash");

    /// <summary>
    /// [ owner? ] <br/>
    /// 사용자가 길드 소유자인지 여부 <br/>
    /// true if the user is the owner of the guild
    /// <summary>
    public bool? IsOwner => Property.FindBooleanOrNull("owner");

    /// [ owner_id ] <br/>
    /// 길드 소유자의 ID <br/>
    /// id of owner
    /// </summary>
    public Snowflake OwnerId => Property.FindSnowflake("owner_id");

    /// <summary>
    /// [ permissions? ] <br/>
    /// 길드 내 사용자 의 총 권한 (덮어쓰기 및 암묵적 권한 제외 ) <br/>
    /// total permissions for the user in the guild (excludes overwrites and implicit permissions)
    /// </summary>
    public string? Permissions 
        => Property.FindStringOrNull("permissions");

    /// <summary>
    /// [ afk_channel_id ] <br/>
    /// AFK 채널 ID <br/>
    /// id of afk channel
    /// </summary>
    public Snowflake? AfkChannelId 
        => Property.FindSnowflakeOrNull("afk_channel_id");

    /// <summary>
    /// [ afk_timeout ] <br/>
    /// AFK 타임아웃 (초) <br/>
    /// afk timeout in seconds
    /// </summary>
    public int AfkTimeout 
        => Property.FindInt("afk_timeout");

    /// <summary>
    /// [ widget_enabled? ] <br/>
    /// 길드 위젯이 활성화되어 있는지 여부 <br/>
    /// true if the server widget is enabled
    public bool? IsWidgetEnabled 
        => Property.FindBooleanOrNull("widget_enabled");

    /// <summary>
    /// [ widget_channel_id? ] <br/>
    /// 길드 위젯 채널 ID <br/>
    /// the channel id that the widget will generate an invite to, or null if set to no invite
    public Snowflake? WidgetChannelId 
        => Property.FindSnowflakeOrNull("widget_channel_id");

    /// <summary>
    /// [ verification_level ] <br/>
    /// 길드의 인증 수준 <br/>
    /// verification level required for the guild
    /// </summary>
    public VerificationLevel VerificationLevel 
        => (VerificationLevel)Property.FindInt("verification_level");

    /// <summary>
    /// [ default_message_notifications ] <br/>
    /// 기본 메시지 알림 수준 <br/>
    /// default message notifications level
    /// </summary>
    public DefaultMessageNotificationLevel DefaultMessageNotifications 
        => (DefaultMessageNotificationLevel)Property.FindInt("default_message_notifications");

    /// <summary>
    /// [ explicit_content_filter ] <br/>
    /// 노골적인 콘텐츠 필터 수준 <br/>
    /// explicit content filter level
    /// </summary>
    public ExplicitContentFilterLevel ExplicitContentFilter 
        => (ExplicitContentFilterLevel)Property.FindInt("explicit_content_filter");

    /// <summary>
    /// [ roles ] <br/>
    /// 길드의 역할 목록 <br/>
    /// roles in the guild
    /// </summary>
    public JsonSourceArraySource<RoleObject> Roles => new(Property.GetProperty("roles"));

    /// <summary>
    /// [ emojis ] <br/>
    /// 사용자 지정 이모지 목록 <br/>
    /// custom guild emojis
    /// </summary>
    public JsonSourceArraySource<EmojiObject> Emojis => new(Property.GetProperty("emojis"));

    /// <summary>
    /// [ features ] <br/>
    /// 활성화된 길드 기능 목록 <br/>
    /// enabled guild features
    /// </summary>
    public StringArraySource Features => Property.FindStringArraySource("features");

    /// <summary>
    /// [ mfa_level ] <br/>
    /// 길드의 MFA 수준 <br/>
    /// required MFA level for the guild
    /// </summary>
    public int MfaLevel => Property.GetProperty("mfa_level").GetInt32();

    /// <summary>
    /// [ application_id? ] <br/>
    /// 봇 생성 시 사용된 애플리케이션 ID <br/>
    /// application id of the guild creator if it is bot-created
    /// </summary>
    public ulong? ApplicationId {
        get {
            JsonElement prop;
            if (Property.TryGetProperty("application_id", out prop) == false || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetUInt64();
        }
    }

    /// <summary>
    /// [ system_channel_id? ] <br/>
    /// 시스템 메시지 채널 ID <br/>
    /// the id of the channel where guild notices such as welcome messages and boost events are posted
    /// </summary>
    public ulong? SystemChannelId {
        get {
            JsonElement prop;
            if (Property.TryGetProperty("system_channel_id", out prop) == false || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetUInt64();
        }
    }

    /// <summary>
    /// [ system_channel_flags ] <br/>
    /// 시스템 채널 플래그 <br/>
    /// system channel flags
    /// </summary>
    public int SystemChannelFlags => Property.GetProperty("system_channel_flags").GetInt32();

    /// <summary>
    /// [ preferred_locale ] <br/>
    /// 커뮤니티 길드의 기본 로케일 <br/>
    /// the preferred locale of a Community guild
    /// </summary>
    public string PreferredLocale => Property.GetProperty("preferred_locale").GetString()!;

    /// <summary>
    /// [ premium_tier ] <br/>
    /// 프리미엄 티어 (서버 부스트 레벨) <br/>
    /// premium tier (Server Boost level)
    /// </summary>
    public int PremiumTier => Property.GetProperty("premium_tier").GetInt32();

    /// <summary>
    /// [ premium_subscription_count? ] <br/>
    /// 현재 길드가 보유한 부스트 수 <br/>
    /// the number of boosts this guild currently has
    /// </summary>
    public int? PremiumSubscriptionCount {
        get {
            JsonElement prop;
            if (Property.TryGetProperty("premium_subscription_count", out prop) == false || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetInt32();
        }
    }

    /// <summary>
    /// [ nsfw_level ] <br/>
    /// NSFW 수준 <br/>
    /// guild NSFW level
    /// </summary>
    public int NsfwLevel => Property.GetProperty("nsfw_level").GetInt32();

    /// <summary>
    /// [ premium_progress_bar_enabled ] <br/>
    /// 부스트 진행 바 활성화 여부 <br/>
    /// whether the guild has the boost progress bar enabled
    /// </summary>
    public bool PremiumProgressBarEnabled => Property.GetProperty("premium_progress_bar_enabled").GetBoolean();

    /// <summary>
    /// [ safety_alerts_channel_id? ] <br/>
    /// 안전 알림 채널 ID <br/>
    /// the id of the channel where admins and moderators of Community guilds receive safety alerts from Discord
    /// </summary>
    public ulong? SafetyAlertsChannelId {
        get {
            JsonElement prop;
            if (Property.TryGetProperty("safety_alerts_channel_id", out prop) == false || prop.ValueKind == JsonValueKind.Null)
                return null;
            return prop.GetUInt64();
        }
    }

    /// <summary>
    /// [ description? ] <br/>
    /// 길드 설명 <br/>
    /// the description of a guild
    /// </summary>
    public string? Description => Property.TryGetProperty("description", out var prop) && prop.ValueKind != JsonValueKind.Null ? prop.GetString() : null;

    /// <summary>
    /// [ banner? ] <br/>
    /// 배너 해시 <br/>
    /// banner hash
    /// </summary>
    public string? Banner => Property.TryGetProperty("banner", out var prop) && prop.ValueKind != JsonValueKind.Null ? prop.GetString() : null;

    /// <summary>
    /// [ vanity_url_code? ] <br/>
    /// 고유 URL 코드 <br/>
    /// the vanity url code for the guild
    /// </summary>
    public string? VanityUrlCode => Property.TryGetProperty("vanity_url_code", out var prop) && prop.ValueKind != JsonValueKind.Null ? prop.GetString() : null;
}