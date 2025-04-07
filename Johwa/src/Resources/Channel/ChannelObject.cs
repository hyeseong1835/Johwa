using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.Channel;

/// <summary>
/// Discord 내의 길드 또는 DM 채널을 나타냅니다. <br/>
/// Represents a guild or DM channel within Discord.
/// </summary>
public struct ChannelObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ChannelObject(JsonElement channelProperty)
    {
        this.Property = channelProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 채널의 ID <br/>
    /// the id of this channel
    /// </summary>
    public ulong Id => Property.GetProperty("id").GetUInt64();

    /// <summary>
    /// [ type ] <br/>
    /// 채널의 타입 <br/>
    /// the type of channel
    /// </summary>
    public int Type => Property.GetProperty("type").GetInt32();

    /// <summary>
    /// [ guild_id? ] <br/>
    /// 채널이 속한 길드 ID <br/>
    /// the id of the guild (may be missing for some channel objects received over gateway guild dispatches)
    /// </summary>
    public ulong? GuildId { get {
        JsonElement prop;
        if (Property.TryGetProperty("guild_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ position? ] <br/>
    /// 채널의 정렬 순서 <br/>
    /// sorting position of the channel (channels with the same position are sorted by id)
    /// </summary>
    public int? Position { get {
        JsonElement prop;
        if (Property.TryGetProperty("position", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetInt32();
    } }

    /// <summary>
    /// [ name? ] <br/>
    /// 채널 이름 <br/>
    /// the name of the channel (1-100 characters)
    /// </summary>
    public string? Name { get {
        JsonElement prop;
        if (Property.TryGetProperty("name", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetString();
    } }

    /// <summary>
    /// [ topic? ] <br/>
    /// 채널 주제 <br/>
    /// the channel topic (0-4096 characters for GUILD_FORUM and GUILD_MEDIA channels, 0-1024 characters for all others)
    /// </summary>
    public string? Topic { get {
        JsonElement prop;
        if (Property.TryGetProperty("topic", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetString();
    } }

    /// <summary>
    /// [ nsfw? ] <br/>
    /// NSFW 여부 <br/>
    /// whether the channel is nsfw
    /// </summary>
    public bool? Nsfw { get {
        JsonElement prop;
        if (Property.TryGetProperty("nsfw", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetBoolean();
    } }

    /// <summary>
    /// [ last_message_id? ] <br/>
    /// 마지막 메시지의 ID <br/>
    /// the id of the last message sent in this channel (or thread for GUILD_FORUM or GUILD_MEDIA channels)
    /// </summary>
    public ulong? LastMessageId { get {
        JsonElement prop;
        if (Property.TryGetProperty("last_message_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ bitrate? ] <br/>
    /// 음성 채널의 비트레이트 <br/>
    /// the bitrate (in bits) of the voice channel
    /// </summary>
    public int? Bitrate { get {
        JsonElement prop;
        if (Property.TryGetProperty("bitrate", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetInt32();
    } }

    /// <summary>
    /// [ user_limit? ] <br/>
    /// 음성 채널의 사용자 제한 수 <br/>
    /// the user limit of the voice channel
    /// </summary>
    public int? UserLimit { get {
        JsonElement prop;
        if (Property.TryGetProperty("user_limit", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetInt32();
    } }

    /// <summary>
    /// [ rate_limit_per_user? ] <br/>
    /// 사용자가 메시지를 보내기 전 대기 시간 (초) <br/>
    /// amount of seconds a user has to wait before sending another message
    /// </summary>
    public int? RateLimitPerUser { get {
        JsonElement prop;
        if (Property.TryGetProperty("rate_limit_per_user", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetInt32();
    } }

    /// <summary>
    /// [ owner_id? ] <br/>
    /// 그룹 DM 또는 스레드의 생성자 ID <br/>
    /// id of the creator of the group DM or thread
    /// </summary>
    public ulong? OwnerId { get {
        JsonElement prop;
        if (Property.TryGetProperty("owner_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ permissions? ] <br/>
    /// 현재 사용자에게 계산된 권한 <br/>
    /// computed permissions for the invoking user in the channel
    /// </summary>
    public string? Permissions { get {
        JsonElement prop;
        if (Property.TryGetProperty("permissions", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetString();
    } }

    /// <summary>
    /// [ flags? ] <br/>
    /// 채널 플래그 비트필드 <br/>
    /// channel flags combined as a bitfield
    /// </summary>
    public ChannelFlag? Flags { get {
        JsonElement prop;
        if (Property.TryGetProperty("flags", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return (ChannelFlag)prop.GetInt32();
    } }
}
