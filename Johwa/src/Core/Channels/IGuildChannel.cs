using System.Text.Json;

namespace Johwa.Core.Channel;

public interface IGuildChannel : IChannel
{
    /// <summary> 
    /// [ GUILD_TEXT, GUILD_VOICE, GUILD_CATEGORY, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 서버(Guild) ID
    /// </summary>
    public ulong? GuildId {
        get {
            JsonElement jsonElement;
            if (Data.TryGetProperty("guild_id", out jsonElement) == false) {
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
            if (Data.TryGetProperty("position", out jsonElement) == false) {
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
    /// [ GUILD_TEXT, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 주제 <br/>
    /// GuildText: 0 ~ 4096자 <br/> 
    /// GUILD_FORUM, GUILD_MEDIA: 0 ~ 1024자
    /// </summary>
    public string? Topic => Data.TryGetProperty("topic", out var v) ? v.GetString() : null;

    /// <summary> NSFW 여부 </summary>
    public bool? IsNsfw => Data.TryGetProperty("nsfw", out var v) ? v.GetBoolean() : null;

    /// <summary> 마지막 메시지 ID </summary>
    public ulong? LastMessageId => Data.TryGetProperty("last_message_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 음성 채널 비트레이트 (bps) </summary>
    public int? Bitrate => Data.TryGetProperty("bitrate", out var v) ? v.GetInt32() : null;

    /// <summary> 음성 채널 사용자 제한 수 </summary>
    public int? UserLimit => Data.TryGetProperty("user_limit", out var v) ? v.GetInt32() : null;

    /// <summary> 사용자 메시지 전송 지연 시간 (초) </summary>
    public int? RateLimitPerUser => Data.TryGetProperty("rate_limit_per_user", out var v) ? v.GetInt32() : null;

    /// <summary> 그룹 DM의 아이콘 해시 </summary>
    public string? Icon => Data.TryGetProperty("icon", out var v) ? v.GetString() : null;

    /// <summary> 그룹 DM 또는 스레드 생성자 ID </summary>
    public ulong? OwnerId => Data.TryGetProperty("owner_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 그룹 DM 생성자의 애플리케이션 ID (봇 생성 시) </summary>
    public ulong? ApplicationId => Data.TryGetProperty("application_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 그룹 DM이 OAuth2 스코프에 의해 관리되는지 여부 </summary>
    public bool? Managed => Data.TryGetProperty("managed", out var v) ? v.GetBoolean() : null;

    /// <summary> 부모 카테고리 채널 ID 또는 스레드의 원본 채널 ID </summary>
    public ulong? ParentId => Data.TryGetProperty("parent_id", out var v) ? v.GetUInt64() : null;

    /// <summary> 마지막 고정 메시지의 시간 </summary>
    public string? LastPinTimestamp => Data.TryGetProperty("last_pin_timestamp", out var v) ? v.GetString() : null;

    /// <summary> 음성 채널의 지역 ID (null이면 자동 설정) </summary>
    public string? RtcRegion => Data.TryGetProperty("rtc_region", out var v) ? v.GetString() : null;

    /// <summary> 비디오 품질 모드 (1: 자동, 2: 720p 고정) </summary>
    public int? VideoQualityMode => Data.TryGetProperty("video_quality_mode", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드 내 메시지 수 (초기 메시지/삭제된 메시지 제외) </summary>
    public int? MessageCount => Data.TryGetProperty("message_count", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드 내 사용자 수 (최대 50까지 카운팅) </summary>
    public int? MemberCount => Data.TryGetProperty("member_count", out var v) ? v.GetInt32() : null;

    /// <summary> 자동 보관 기간 (스레드 생성 시 기본값, 분 단위) </summary>
    public int? DefaultAutoArchiveDuration => Data.TryGetProperty("default_auto_archive_duration", out var v) ? v.GetInt32() : null;

    /// <summary> 명령어 인터랙션 시 호출자의 권한 비트필드 문자열 </summary>
    public string? Permissions => Data.TryGetProperty("permissions", out var v) ? v.GetString() : null;

    /// <summary> 채널 플래그 (비트필드) </summary>
    public int? Flags => Data.TryGetProperty("flags", out var v) ? v.GetInt32() : null;

    /// <summary> 스레드에서 지금까지 보낸 전체 메시지 수 </summary>
    public int? TotalMessageSent => Data.TryGetProperty("total_message_sent", out var v) ? v.GetInt32() : null;

    /// <summary> 포럼/미디어 채널의 기본 정렬 방식 </summary>
    public int? DefaultSortOrder => Data.TryGetProperty("default_sort_order", out var v) ? v.GetInt32() : null;

    /// <summary> 포럼 채널의 기본 레이아웃 방식 </summary>
    public int? DefaultForumLayout => Data.TryGetProperty("default_forum_layout", out var v) ? v.GetInt32() : null;
}
