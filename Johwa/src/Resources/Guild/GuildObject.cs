using System.Text.Json;
using Johwa.Common;
using Johwa.Common.JsonSource;
using Johwa.Utility;
using Johwa.Utility.Json;

namespace Johwa.Resources.Guild;

public struct GuildObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public GuildObject(JsonElement guildProperty)
    {
        Property = guildProperty;
    }

    /// <summary>
    /// [ id (snowflake) ] <br/>
    /// 길드 ID <br/>
    /// guild id
    /// </summary>
    public Snowflake Id 
        => Property.FindSnowflake("id");

    /// <summary>
    /// [ name (string) ] <br/>
    /// 길드 이름 <br/>
    /// guild name (2-100 characters, excluding trailing and leading whitespace)
    /// </summary>
    public string Name 
        => Property.FindString("name");

    /// <summary>
    /// [ icon (?string) ] <br/>
    /// 아이콘 해시 <br/>
    /// icon hash
    /// </summary>
    public string? Icon 
        => Property.FindNullableString("icon");

    /// <summary>
    /// [ splash (?string) ] <br/>
    /// 스플래시 해시 <br/>
    /// splash hash
    /// </summary>
    public string? Splash 
        => Property.FindNullableString("splash");
    
    /// <summary>
    /// [ discovery_splash (?string) ] <br/>
    /// 디스커버리 스플래시 해시 <br/>
    /// discovery splash hash
    /// </summary>
    public string? DiscoverySplash 
        => Property.FindNullableString("discovery_splash");

    /// [ owner_id (snowflake) ] <br/>
    /// 길드 소유자의 ID <br/>
    /// id of owner
    /// </summary>
    public Snowflake OwnerId 
        => Property.FindSnowflake("owner_id");

    /// <summary>
    /// [ afk_channel_id (?snowflake) ] <br/>
    /// AFK 채널 ID <br/>
    /// id of afk channel
    /// </summary>
    public Snowflake? AfkChannelId 
        => Property.FindNullableSnowflake("afk_channel_id");

    /// <summary>
    /// [ afk_timeout (integer) ] <br/>
    /// AFK 타임아웃 (초) <br/>
    /// afk timeout in seconds
    /// </summary>
    public int AfkTimeout 
        => Property.FindInt("afk_timeout");

    /// <summary>
    /// [ widget_enabled? (boolean) ] <br/>
    /// 길드 위젯이 활성화되어 있는지 여부 <br/>
    /// true if the server widget is enabled
    public bool? IsWidgetEnabled 
        => Property.FindBooleanOrNull("widget_enabled");

    /// <summary>
    /// [ widget_channel_id? (?snowflake) ] <br/>
    /// 길드 위젯 채널 ID <br/>
    /// the channel id that the widget will generate an invite to, or null if set to no invite
    public Snowflake? WidgetChannelId 
        => Property.FindNullableSnowflakeOrNull("widget_channel_id");

    /// <summary>
    /// [ verification_level (integer) ] <br/>
    /// 길드의 인증 수준 <br/>
    /// verification level required for the guild
    /// </summary>
    public VerificationLevel VerificationLevel 
        => (VerificationLevel)Property.FindInt("verification_level");

    /// <summary>
    /// [ default_message_notifications (integer) ] <br/>
    /// 기본 메시지 알림 수준 <br/>
    /// default message notifications level
    /// </summary>
    public DefaultMessageNotificationLevel DefaultMessageNotifications 
        => (DefaultMessageNotificationLevel)Property.FindInt("default_message_notifications");

    /// <summary>
    /// [ explicit_content_filter (integer) ] <br/>
    /// 노골적인 콘텐츠 필터 수준 <br/>
    /// explicit content filter level
    /// </summary>
    public ExplicitContentFilterLevel ExplicitContentFilter 
        => (ExplicitContentFilterLevel)Property.FindInt("explicit_content_filter");

    /// <summary>
    /// [ roles (array of role objects) ] <br/>
    /// 길드의 역할 목록 <br/>
    /// roles in the guild
    /// </summary>
    public JsonSourceArraySource<RoleObject> Roles 
        => Property.FindJsonSourceArraySource<RoleObject>("roles");

    /// <summary>
    /// [ emojis (array of emoji objects) ] <br/>
    /// 사용자 지정 이모지 목록 <br/>
    /// custom guild emojis
    /// </summary>
    //public JsonSourceArraySource<EmojiObject> Emojis => new(Property.GetProperty("emojis"));

    /// <summary>
    /// [ features (array of guild feature strings) ] <br/>
    /// 활성화된 길드 기능 목록 <br/>
    /// enabled guild features
    /// </summary>
    public StringArraySource Features => Property.FindStringArraySource("features");

    /// <summary>
    /// [ mfa_level (integer) ] <br/>
    /// 길드의 MFA 수준 <br/>
    /// required MFA level for the guild
    /// </summary>
    public int MfaLevel => Property.GetProperty("mfa_level").GetInt32();

    /// <summary>
    /// [ application_id (?snowflake) ] <br/>
    /// 봇 생성 시 사용된 애플리케이션 ID <br/>
    /// application id of the guild creator if it is bot-created
    /// </summary>
    public Snowflake? ApplicationId 
        => Property.FindNullableSnowflake("application_id");

    /// <summary>
    /// [ system_channel_id (?snowflake) ] <br/>
    /// 시스템 메시지 채널 ID <br/>
    /// the id of the channel where guild notices such as welcome messages and boost events are posted
    /// </summary>
    public Snowflake? SystemChannelId 
        => Property.FindNullableSnowflake("system_channel_id");

    /// <summary>
    /// [ system_channel_flags (integer) ] <br/>
    /// 시스템 채널 플래그 <br/>
    /// system channel flags
    /// </summary>
    public SystemChannelFlag SystemChannelFlags
        => (SystemChannelFlag)Property.FindInt("system_channel_flags");

    /// <summary>
    /// [ rules_channel_id (?snowflake) ] <br/>
    /// 커뮤니티 길드의 규칙 및 가이드라인을 표시할 수 있는 채널 ID <br/>
    /// the id of the channel where Community guilds can display rules and/or guidelines
    /// </summary>
    public Snowflake? RulesChannelId 
        => Property.FindNullableSnowflake("rules_channel_id");

    /// <summary>
    /// [ max_presences? (?integer) ] <br/>
    /// 길드의 최대 Presences 수 (null은 항상 반환됨, 대규모 길드 제외) <br/>
    /// the maximum number of presences for the guild (null is always returned, apart from the largest of guilds)
    /// </summary>
    public int? MaxPresences
        => Property.FindNullableIntOrNull("max_presences");

    /// <summary>
    /// [ max_members? (?integer) ] <br/>
    /// 길드의 최대 멤버 수
    /// the maximum number of members for the guild
    /// </summary>
    public int? MaxMembers
        => Property.FindIntOrNull("max_members");

    /// <summary>
    /// [ vanity_url_code? (?string) ] <br/>
    /// 길드에서 고유한 URL 코드 <br/>
    /// the vanity url code for the guild
    /// </summary>
    public string? VanityUrlCode 
        => Property.FindNullableStringOrNull("vanity_url_code");

    /// <summary>
    /// [ description (?string) ] <br/>
    /// 길드 설명 <br/>
    /// the description of a guild
    /// </summary>
    public string? Description 
        => Property.FindNullableString("description");

    /// <summary>
    /// [ banner (?string) ] <br/>
    /// 배너 해시 <br/>
    /// banner hash
    /// </summary>
    public string? Banner 
        => Property.FindNullableString("banner");

    /// <summary>
    /// [ premium_tier (integer) ] <br/>
    /// 프리미엄 티어 (서버 부스트 레벨) <br/>
    /// premium tier (Server Boost level)
    /// </summary>
    public PremiumTier PremiumTier 
        => (PremiumTier)Property.FindInt("premium_tier");

    /// <summary>
    /// [ premium_subscription_count? (integer) ] <br/>
    /// 현재 길드가 보유한 부스트 수 <br/>
    /// the number of boosts this guild currently has
    /// </summary>
    public int? PremiumSubscriptionCount 
        => Property.FindIntOrNull("premium_subscription_count");

    /// <summary>
    /// [ preferred_locale (string) ] <br/>
    /// 커뮤니티 길드의 기본 로케일 <br/>
    /// the preferred locale of a Community guild
    /// </summary>
    public string PreferredLocale 
        => Property.GetProperty("preferred_locale").GetString()!;

    // public_updates_channel_id	?snowflake	the id of the channel where admins and moderators of Community guilds receive notices from Discord
    /// <summary>
    /// [ public_updates_channel_id (?snowflake) ] <br/>
    /// 커뮤니티 길드의 공지 채널 ID <br/>
    /// the id of the channel where admins and moderators of Community guilds receive notices from Discord
    /// </summary>
    public Snowflake? PublicUpdatesChannelId 
        => Property.FindNullableSnowflake("public_updates_channel_id");
    
    /// <summary>
    /// [ max_video_channel_users? (?integer) ] <br/>
    /// 비디오 채널의 최대 사용자 수 <br/>
    /// the maximum amount of users in a video channel
    /// </summary>
    public int? MaxVideoChannelUsers 
        => Property.FindNullableIntOrNull("max_video_channel_users");

    /// <summary>
    /// [max_stage_video_channel_users? (?integer) ] <br/>
    /// 스테이지 비디오 채널의 최대 사용자 수 <br/>
    /// the maximum amount of users in a stage video channel
    /// </summary>
    public int? MaxStageVideoChannelUsers 
        => Property.FindNullableIntOrNull("max_stage_video_channel_users");

    /// <summary>
    /// [ approximate_presence_count? (integer) ] <br/>
    /// 대략적인 프레즌스 수 <br/>
    /// approximate number of non-offline members in this guild, returned from the GET /guilds/<id> and /users/@me/guilds endpoints when with_counts is true
    /// </summary>
    public int? ApproximatePresenceCount 
        => Property.FindIntOrNull("approximate_presence_count");

    /// <summary>
    /// [ welcome_screen? (welcome screen object) ] <br/>
    /// 새로운 멤버에게 표시되는 커뮤니티 길드의 환영 화면, 초대된 길드 객체로 반환됩니다.
    /// the welcome screen of a Community guild, shown to new members, returned in an Invite's guild object
    /// </summary>
    public WelcomeScreenObject? WelcomeScreen 
        => Property.FindJsonSourceOrNull<WelcomeScreenObject>("welcome_screen");
    
    // stickers?	array of sticker objects	
    /// <summary>
    /// [ stickers? (array of sticker objects) ] <br/>
    /// 커스텀 길드 스티커 목록 <br/>
    /// custom guild stickers
    /// </summary>
    public JsonSourceArraySource<StickerObject> Stickers 
        => Property.FindJsonSourceArraySource<StickerObject>("stickers");

    /// <summary>
    /// [ safety_alerts_channel_id (?snowflake) ] <br/>
    /// 커뮤니티 길드의 관리자와 감독자가 Discord로부터 안전 알림을 받는 채널의 ID <br/>
    /// the id of the channel where admins and moderators of Community guilds receive safety alerts from Discord
    /// </summary>
    public Snowflake? SafetyAlertsChannelId 
        => Property.FindNullableSnowflake("safety_alerts_channel_id");

    /// <summary>
    /// [ nsfw_level (integer) ] <br/>
    /// 길드의 NSFW 수준 <br/>
    /// guild NSFW level
    /// </summary>
    public NsfwLevel NsfwLevel 
        => (NsfwLevel)Property.FindInt("nsfw_level");

    /// <summary>
    /// [ premium_progress_bar_enabled (boolean) ] <br/>
    /// 부스트 진행 바 활성화 여부 <br/>
    /// whether the guild has the boost progress bar enabled
    /// </summary>
    public bool PremiumProgressBarEnabled 
        => Property.FindBoolean("premium_progress_bar_enabled");

    /// <summary>
    /// [ safety_alerts_channel_id (?snowflake) ] <br/>
    /// 안전 알림 채널 ID <br/>
    /// the id of the channel where admins and moderators of Community guilds receive safety alerts from Discord
    /// </summary>
    public Snowflake? SafetyAlertsChannelId 
        => Property.FindNullableSnowflake("safety_alerts_channel_id");
    
    /// <summary>
    /// [ incidents_data (?incidents data object) ] <br/>
    /// 길드의 사고 데이터 <br/>
    /// the incidents data for this guild
    /// </summary>
    public IncidentsDataObject? IncidentsData 
        => Property.FindJsonSourceOrNull<IncidentsDataObject>("incidents_data");
}