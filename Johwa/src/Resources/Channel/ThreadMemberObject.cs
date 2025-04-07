using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.Channel;

/// <summary>
/// 스레드에 참여한 사용자에 대한 정보를 포함합니다. <br/>
/// A thread member object contains information about a user that has joined a thread.
/// </summary>
public struct ThreadMemberObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ThreadMemberObject(JsonElement threadMemberProperty)
    {
        this.Property = threadMemberProperty;
    }

    /// <summary>
    /// [ id? ] <br/>
    /// 스레드의 ID <br/>
    /// ID of the thread
    /// </summary>
    public ulong? Id { get {
        JsonElement prop;
        if (Property.TryGetProperty("id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ user_id? ] <br/>
    /// 사용자의 ID <br/>
    /// ID of the user
    /// </summary>
    public ulong? UserId { get {
        JsonElement prop;
        if (Property.TryGetProperty("user_id", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.String)
            return ulong.TryParse(prop.GetString(), out var id) ? id : null;

        if (prop.ValueKind == JsonValueKind.Number)
            return prop.GetUInt64();

        return null;
    } }

    /// <summary>
    /// [ join_timestamp ] <br/>
    /// 사용자가 마지막으로 스레드에 참여한 시간 <br/>
    /// Time the user last joined the thread
    /// </summary>
    public DateTime? JoinTimestamp { get {
        JsonElement prop;
        if (Property.TryGetProperty("join_timestamp", out prop) == false)
            return null;

        return DateTime.TryParse(prop.GetString(), out var time) ? time : null;
    } }

    /// <summary>
    /// [ flags ] <br/>
    /// 사용자-스레드 설정 값 (현재는 알림에만 사용됨) <br/>
    /// Any user-thread settings, currently only used for notifications
    /// </summary>
    public int Flags => Property.GetProperty("flags").GetInt32();

    /// <summary>
    /// [ member? ] <br/>
    /// 사용자에 대한 추가 정보 <br/>
    /// Additional information about the user
    /// </summary>
    public GuildMember? Member { get {
        JsonElement prop;
        if (Property.TryGetProperty("member", out prop) == false)
            return null;
        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return new GuildMember(prop);
    } }
}
