using System.Text.Json;

namespace Johwa.Core.Channel;

/// <summary>
/// Discord의 채널(Channel) 객체를 표현하는 구조체입니다.
/// </summary>
public struct Channel
{
    public readonly JsonElement data;

    /// <summary> 
    /// [ 모두 ] <br/>
    /// 채널 ID (Snowflake) 
    /// </summary>
    public ulong Id => data.GetProperty("id").GetUInt64();

    /// <summary> 
    /// [ 모두 ] <br/>
    /// 채널 유형
    /// </summary>
    public ChannelType Type => (ChannelType)data.GetProperty("type").GetInt32();

    /// <summary> 
    /// [ GUILD_TEXT, GUILD_VOICE, GUILD_CATEGORY, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 서버(Guild) ID
    /// </summary>
    public ulong? GuildId {
        get {
            JsonElement jsonElement;
            if (data.TryGetProperty("guild_id", out jsonElement) == false) {
                return null;
            }
            if (jsonElement.ValueKind == JsonValueKind.String) {
                return ulong.TryParse(jsonElement.GetString(), out var id) ? id : null;
            } 
            else if (jsonElement.ValueKind == JsonValueKind.Number) {
                return jsonElement.GetUInt64();
            }
            else return null;
        }
    }
    
    /// <summary> 
    /// [ GUILD_TEXT, GUILD_VOICE, GUILD_CATEGORY, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 정렬 위치 
    /// </summary>
    public int? Position {
        get {
            JsonElement jsonElement;
            if (data.TryGetProperty("position", out jsonElement) == false) {
                return null;
            }
            if (jsonElement.ValueKind == JsonValueKind.String) {
                return int.TryParse(jsonElement.GetString(), out var id)? id : null;
            } 
            else if (jsonElement.ValueKind == JsonValueKind.Number) {
                return jsonElement.GetInt32();
            }
            else return null;
        }
    }

    /// <summary> 
    /// [ GUILD_TEXT, DM, GUILD_VOICE, GROUP_DM, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 이름 
    /// </summary>
    public string? Name => data.TryGetProperty("name", out var v) ? v.GetString() : null;

    /// <summary> 
    /// [ GUILD_TEXT, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 주제 <br/>
    /// GuildText: 0 ~ 4096자 <br/> 
    /// GUILD_FORUM, GUILD_MEDIA: 0 ~ 1024자
    /// </summary>
    public string? Topic => data.TryGetProperty("topic", out var v) ? v.GetString() : null;

    /// <summary> NSFW 여부 </summary>
    public bool? IsNsfw => data.TryGetProperty("nsfw", out var v) ? v.GetBoolean() : null;

    /// <summary> 마지막 메시지 ID </summary>
    public ulong? LastMessageId => data.TryGetProperty("last_message_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 음성 채널 비트레이트 (bps) </summary>
    public int? Bitrate => data.TryGetProperty("bitrate", out var v) ? v.GetInt32() : null;

    /// <summary> 음성 채널 사용자 제한 수 </summary>
    public int? UserLimit => data.TryGetProperty("user_limit", out var v) ? v.GetInt32() : null;

    /// <summary> 사용자 메시지 전송 지연 시간 (초) </summary>
    public int? RateLimitPerUser => data.TryGetProperty("rate_limit_per_user", out var v) ? v.GetInt32() : null;

    /// <summary> 그룹 DM의 아이콘 해시 </summary>
    public string? Icon => data.TryGetProperty("icon", out var v) ? v.GetString() : null;

    /// <summary> 그룹 DM 또는 스레드 생성자 ID </summary>
    public ulong? OwnerId => data.TryGetProperty("owner_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 그룹 DM 생성자의 애플리케이션 ID (봇 생성 시) </summary>
    public ulong? ApplicationId => data.TryGetProperty("application_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 그룹 DM이 OAuth2 스코프에 의해 관리되는지 여부 </summary>
    public bool? Managed => data.TryGetProperty("managed", out var v) ? v.GetBoolean() : null;

    /// <summary> 부모 카테고리 채널 ID 또는 스레드의 원본 채널 ID </summary>
    public ulong? ParentId => data.TryGetProperty("parent_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 마지막 고정 메시지의 시간 </summary>
    public string? LastPinTimestamp => data.TryGetProperty("last_pin_timestamp", out var v) ? v.GetString() : null;

    /// <summary> 음성 채널의 지역 ID (null이면 자동 설정) </summary>
    public string? RtcRegion => data.TryGetProperty("rtc_region", out var v) ? v.GetString() : null;

    /// <summary> 비디오 품질 모드 (1: 자동, 2: 720p 고정) </summary>
    public int? VideoQualityMode => data.TryGetProperty("video_quality_mode", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드 내 메시지 수 (초기 메시지/삭제된 메시지 제외) </summary>
    public int? MessageCount => data.TryGetProperty("message_count", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드 내 사용자 수 (최대 50까지 카운팅) </summary>
    public int? MemberCount => data.TryGetProperty("member_count", out var v) ? v.GetInt32() : null;

    /// <summary> 자동 보관 기간 (스레드 생성 시 기본값, 분 단위) </summary>
    public int? DefaultAutoArchiveDuration => data.TryGetProperty("default_auto_archive_duration", out var v) ? v.GetInt32() : null;

    /// <summary> 명령어 인터랙션 시 호출자의 권한 비트필드 문자열 </summary>
    public string? Permissions => data.TryGetProperty("permissions", out var v) ? v.GetString() : null;

    /// <summary> 채널 플래그 (비트필드) </summary>
    public int? Flags => data.TryGetProperty("flags", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드에서 지금까지 보낸 전체 메시지 수 </summary>
    public int? TotalMessageSent => data.TryGetProperty("total_message_sent", out var v) ? v.GetInt32() : null;

    /// <summary> 포럼/미디어 채널의 기본 정렬 방식 </summary>
    public int? DefaultSortOrder => data.TryGetProperty("default_sort_order", out var v) ? v.GetInt32() : null;

    /// <summary> 포럼 채널의 기본 레이아웃 방식 </summary>
    public int? DefaultForumLayout => data.TryGetProperty("default_forum_layout", out var v) ? v.GetInt32() : null;
    /// <summary>
    /// DM 채널일 경우 수신자 목록 (User 객체 배열)
    /// </summary>
    public JsonElement? Recipients => data.TryGetProperty("recipients", out var v) ? v : null;

    /// <summary>
    /// 권한 오버라이드 목록 (Permission Overwrite 객체 배열)
    /// </summary>
    public JsonElement? PermissionOverwrites => data.TryGetProperty("permission_overwrites", out var v) ? v : null;

    /// <summary>
    /// 스레드의 메타데이터 (보관 설정 등)
    /// </summary>
    public JsonElement? ThreadMetadata => data.TryGetProperty("thread_metadata", out var v) ? v : null;

    /// <summary>
    /// 현재 사용자가 스레드에 참여 중인 경우에만 존재하는 thread member 객체
    /// </summary>
    public JsonElement? Member => data.TryGetProperty("member", out var v) ? v : null;

    /// <summary>
    /// 포럼 또는 미디어 채널에서 사용할 수 있는 태그 목록
    /// </summary>
    public JsonElement? AvailableTags => data.TryGetProperty("available_tags", out var v) ? v : null;

    /// <summary>
    /// 포럼 또는 미디어 채널 스레드에 적용된 태그 ID 목록
    /// </summary>
    public JsonElement? AppliedTags => data.TryGetProperty("applied_tags", out var v) ? v : null;

    /// <summary>
    /// 스레드에 사용할 기본 반응 이모지 (Add Reaction 버튼에 표시)
    /// </summary>
    public JsonElement? DefaultReactionEmoji => data.TryGetProperty("default_reaction_emoji", out var v) ? v : null;

    /// <summary>
    /// 새로 생성된 스레드에 기본 적용되는 rate_limit_per_user (초 단위)
    /// </summary>
    public int? DefaultThreadRateLimitPerUser => data.TryGetProperty("default_thread_rate_limit_per_user", out var v) ? v.GetInt32() : null;

    public Channel(JsonElement data)
    {
        this.data = data;
    }
}
