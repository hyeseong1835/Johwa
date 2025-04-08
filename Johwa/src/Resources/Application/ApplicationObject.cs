using System.Text.Json;
using Johwa.Common;
using Johwa.Common.JsonSource;
using Johwa.Utility;
using Johwa.Utility.Json;

namespace Johwa.Resources.Application;

/// <summary>
/// Discord 개발자 플랫폼 기능을 포함하는 앱 정보 객체 <br/>
/// Applications (or "apps") are containers for developer platform features, and can be installed to Discord servers and/or user accounts.
/// </summary>
public struct ApplicationObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ApplicationObject(JsonElement applicationProperty)
    {
        this.Property = applicationProperty;
    }

    /// <summary>
    /// [ id (snowflake) ] <br/>
    /// 앱 ID <br/>
    /// ID of the app
    /// </summary>
    public Snowflake Id => Property.FindSnowflake("id");

    /// <summary>
    /// [ name (string) ]
    /// 앱 이름 <br/>
    /// Name of the app
    /// </summary>
    public string Name 
        => Property.FindString("name");

    /// <summary>
    /// [ icon (string?) ] <br/>
    /// 앱 아이콘 해시 <br/>
    /// Icon hash of the app
    /// </summary>
    public string? Icon 
        => Property.FindNullableString("icon");

    /// <summary>
    /// [ icon_hash (string) ] <br/>
    /// 앱 설명 <br/>
    /// Description of the app
    /// </summary>
    public string Description 
        => Property.FindString("description");

    /// <summary>
    /// [ rpc_origins? (array of strings) ] <br/>
    /// RPC가 활성화된 경우 RPC 원본 URL 목록 <br/>
    /// List of RPC origin URLs, if RPC is enabled
    /// </summary>
    public StringArraySource? RpcOrigins
        => Property.FindStringArraySourceOrNull("rpc_origins");

    /// <summary>
    /// /// [ bot_public (boolean) ] <br/>
    /// 공개 앱 여부 <br/>
    /// When false, only the app owner can add the app to guilds
    /// </summary>
    public bool IsBotPublic 
        => Property.FindBoolean("bot_public");

    /// <summary>
    /// [ bot_require_code_grant (boolean) ] <br/>
    /// 전체 OAuth2 코드 그랜트 플로우가 완료되어야 봇이 참여 가능 여부 <br/>
    /// When true, the app's bot will only join upon completion of the full OAuth2 code grant flow
    /// </summary>
    public bool IsBotRequireCodeGrant 
        => Property.FindBoolean("bot_require_code_grant");

    /// <summary>
    /// [ bot? (partial user object) ] <br/>
    /// 앱에 연결된 봇의 Partial User 객체 <br/>
    /// Partial user object for the bot user associated with the app
    /// </summary>
    public PartialUser? Bot 
        => Property.FindJsonSourceOrNull<PartialUser>("bot");

    /// <summary>
    /// [ terms_of_service_url? (string) ] <br/>
    /// 서비스 약관 URL <br/>
    /// URL of the app's Terms of Service
    /// </summary>
    public string? TermsOfServiceUrl 
        => Property.FindStringOrNull("terms_of_service_url");

    /// <summary>
    /// [ privacy_policy_url? (string) ] <br/>
    /// 개인정보처리방침 URL <br/>
    /// URL of the app's Privacy Policy
    /// </summary>
    public string? PrivacyPolicyUrl 
        => Property.FindStringOrNull("privacy_policy_url");

    /// <summary>
    /// [ owner? (partial user object) ] <br/>
    /// 앱 소유자의 Partial User 객체 <br/>
    /// Partial user object for the owner of the app
    /// </summary>
    //public PartialUser? Owner 
    //    => Property.FindJsonSourceOrNull<PartialUser>("owner");

    /// <summary>
    /// [ verify_key (string) ] <br/>
    /// 상호작용 검증용 키 <br/>
    /// Hex encoded key for verification in interactions and the GameSDK's GetTicket
    /// </summary>
    public string VerifyKey
        => Property.FindString("verify_key");

    /// <summary>
    /// [ team? (team object?)] <br/>
    /// 앱이 팀에 속해 있는 경우, 팀 정보 객체 <br/>
    /// If the app belongs to a team, this will be a list of the members of that team
    /// </summary>
    //public TeamObject? Team
    //    => Property.FindNullableJsonSourceOrNull<TeamObject>("team");

    /// <summary>
    /// [ guild_id? ] <br/>
    /// 앱과 연결된 길드 ID <br/>
    /// Guild associated with the app. For example, a developer support server.
    /// </summary>
    public ulong? GuildId { get {
        JsonElement prop;
        if (Property.TryGetProperty("guild_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        if (prop.ValueKind == JsonValueKind.String){
            return ulong.TryParse(prop.GetString(), out ulong id) ? id : null;
        }

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ guild? ] <br/>
    /// 앱과 연결된 Partial Guild 객체 <br/>
    /// Partial object of the associated guild
    /// </summary>
    //public PartialGuild? Guild
    //    => Property.FindJsonSourceOrNull<PartialGuild>("guild");

    /// <summary>
    /// [ primary_sku_id? (snowflake) ] <br/>
    /// 앱이 Discord에서 판매되는 게임인 경우 생성된 게임 SKU ID <br/>
    /// If this app is a game sold on Discord, this field will be the id of the "Game SKU" that is created, if exists
    /// </summary>
    public Snowflake? PrimarySkuId
        => Property.FindSnowflakeOrNull("primary_sku_id");

    /// <summary>
    /// [ slug? (string) ] <br/>
    /// Discord에서 판매되는 게임 앱의 스토어 링크용 URL 슬러그 <br/>
    /// If this app is a game sold on Discord, this field will be the URL slug that links to the store page
    /// </summary>
    public string? Slug
        => Property.FindStringOrNull("slug");

    /// <summary>
    /// [ cover_image? (string) ] <br/>
    /// 앱의 기본 리치 프레즌스 초대 커버 이미지 해시 <br/>
    /// App's default rich presence invite cover image hash
    /// </summary>
    public string? CoverImage
        => Property.FindStringOrNull("cover_image");

    /// <summary>
    /// [ flags? (integer) ] <br/>
    /// 앱의 공개 플래그 <br/>
    /// App's public flags
    /// </summary>
    public int? Flags
        => Property.FindNullableInt("flags");

    /// <summary>
    /// [ approximate_guild_count? ] <br/>
    /// 앱이 추가된 서버의 예상 개수 <br/>
    /// Approximate count of guilds the app has been added to
    /// </summary>
    public int? ApproximateGuildCount
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("approximate_guild_count", out prop) == false)
                return null;
            return prop.GetInt32();
        }
    }

    /// <summary>
    /// [ approximate_user_install_count? ] <br/>
    /// 앱을 설치한 유저의 예상 개수 <br/>
    /// Approximate count of users that have installed the app
    /// </summary>
    public int? ApproximateUserInstallCount
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("approximate_user_install_count", out prop) == false)
                return null;
            return prop.GetInt32();
        }
    }

    /// <summary>
    /// [ interactions_endpoint_url? ] <br/>
    /// 상호작용 엔드포인트 URL <br/>
    /// Interactions endpoint URL for the app
    /// </summary>
    public string? InteractionsEndpointUrl
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("interactions_endpoint_url", out prop) == false)
                return null;
            return prop.GetString();
        }
    }

    /// <summary>
    /// [ role_connections_verification_url? ] <br/>
    /// 역할 연결 검증용 URL <br/>
    /// Role connection verification URL for the app
    /// </summary>
    public string? RoleConnectionsVerificationUrl
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("role_connections_verification_url", out prop) == false)
                return null;
            return prop.GetString();
        }
    }

    /// <summary>
    /// [ event_webhooks_url? ] <br/>
    /// 웹훅 이벤트 수신용 URL <br/>
    /// Event webhooks URL for the app to receive webhook events
    /// </summary>
    public string? EventWebhooksUrl
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("event_webhooks_url", out prop) == false)
                return null;
            return prop.GetString();
        }
    }

    /// <summary>
    /// 웹훅 이벤트 사용 상태 <br/>
    /// If webhook events are enabled for the app
    /// </summary>
    public int EventWebhooksStatus => Property.GetProperty("event_webhooks_status").GetInt32();
}
