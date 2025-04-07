using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.Voice;

/// <summary>
/// 사용자의 음성 연결 상태를 나타내는 데 사용됩니다. <br/>
/// Used to represent a user's voice connection status.
/// </summary>
public struct VoiceStateObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public VoiceStateObject(JsonElement voiceStateProperty)
    {
        this.Property = voiceStateProperty;
    }

    /// <summary>
    /// [ guild_id? ] <br/>
    /// 이 Voice State가 속한 길드 ID <br/>
    /// the guild id this voice state is for
    /// </summary>
    public ulong? GuildId { get {
        JsonElement prop;
        if (Property.TryGetProperty("guild_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ channel_id? ] <br/>
    /// 사용자가 연결된 채널 ID <br/>
    /// the channel id this user is connected to
    /// </summary>
    public ulong? ChannelId { get {
        JsonElement prop;
        if (Property.TryGetProperty("channel_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ user_id ] <br/>
    /// 해당 Voice State의 사용자 ID <br/>
    /// the user id this voice state is for
    /// </summary>
    public ulong UserId => Property.GetProperty("user_id").GetUInt64();

    /// <summary>
    /// [ member? ] <br/>
    /// 해당 Voice State의 길드 멤버 정보 <br/>
    /// the guild member this voice state is for
    /// </summary>
    public GuildMemberObject? Member { get {
        JsonElement prop;
        if (Property.TryGetProperty("member", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return new GuildMemberObject(prop);
    } }

    /// <summary>
    /// [ session_id ] <br/>
    /// 해당 Voice State의 세션 ID <br/>
    /// the session id for this voice state
    /// </summary>
    public string SessionId => Property.GetProperty("session_id").GetString()!;

    /// <summary>
    /// [ deaf ] <br/>
    /// 서버에서 사용자를 청각 제한했는지 여부 <br/>
    /// whether this user is deafened by the server
    /// </summary>
    public bool Deaf => Property.GetProperty("deaf").GetBoolean();

    /// <summary>
    /// [ mute ] <br/>
    /// 서버에서 사용자를 음소거했는지 여부 <br/>
    /// whether this user is muted by the server
    /// </summary>
    public bool Mute => Property.GetProperty("mute").GetBoolean();

    /// <summary>
    /// [ self_deaf ] <br/>
    /// 사용자가 로컬에서 본인을 청각 제한했는지 여부 <br/>
    /// whether this user is locally deafened
    /// </summary>
    public bool SelfDeaf => Property.GetProperty("self_deaf").GetBoolean();

    /// <summary>
    /// [ self_mute ] <br/>
    /// 사용자가 로컬에서 본인을 음소거했는지 여부 <br/>
    /// whether this user is locally muted
    /// </summary>
    public bool SelfMute => Property.GetProperty("self_mute").GetBoolean();

    /// <summary>
    /// [ self_stream? ] <br/>
    /// 사용자가 "Go Live"로 스트리밍 중인지 여부 <br/>
    /// whether this user is streaming using "Go Live"
    /// </summary>
    public bool? SelfStream { get {
        JsonElement prop;
        if (Property.TryGetProperty("self_stream", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetBoolean();
    } }

    /// <summary>
    /// [ self_video ] <br/>
    /// 사용자가 카메라를 켰는지 여부 <br/>
    /// whether this user's camera is enabled
    /// </summary>
    public bool SelfVideo => Property.GetProperty("self_video").GetBoolean();

    /// <summary>
    /// [ suppress ] <br/>
    /// 사용자의 발언 권한이 제한되었는지 여부 <br/>
    /// whether this user's permission to speak is denied
    /// </summary>
    public bool Suppress => Property.GetProperty("suppress").GetBoolean();

    /// <summary>
    /// [ request_to_speak_timestamp? ] <br/>
    /// 사용자가 발언 요청을 한 시간 <br/>
    /// the time at which the user requested to speak
    /// </summary>
    public DateTime? RequestToSpeakTimestamp { get {
        JsonElement prop;
        if (Property.TryGetProperty("request_to_speak_timestamp", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return DateTime.TryParse(prop.GetString(), out var dt) ? dt : null;
    } }
}