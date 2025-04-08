using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.Channel;

/// <summary>
/// GUILD_FORUM 또는 GUILD_MEDIA 채널에서 스레드에 적용할 수 있는 태그를 나타내는 객체입니다. <br/>
/// An object that represents a tag that is able to be applied to a thread in a GUILD_FORUM or GUILD_MEDIA channel.
/// </summary>
public struct ForumTagObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ForumTagObject(JsonElement forumTagProperty)
    {
        this.Property = forumTagProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 태그의 ID <br/>
    /// the id of the tag
    /// </summary>
    public ulong Id => Property.GetProperty("id").GetUInt64();

    /// <summary>
    /// [ name ] <br/>
    /// 태그의 이름 (0~20자) <br/>
    /// the name of the tag (0-20 characters)
    /// </summary>
    public string Name => Property.GetProperty("name").GetString()!;

    /// <summary>
    /// [ moderated ] <br/>
    /// MANAGE_THREADS 권한이 있는 멤버만 이 태그를 스레드에 추가/제거할 수 있는지 여부 <br/>
    /// whether this tag can only be added to or removed from threads by a member with the MANAGE_THREADS permission
    /// </summary>
    public bool Moderated => Property.GetProperty("moderated").GetBoolean();

    /// <summary>
    /// [ emoji_id? ] <br/>
    /// 길드의 커스텀 이모지 ID <br/>
    /// the id of a guild's custom emoji
    /// </summary>
    public ulong? EmojiId { get {
        JsonElement prop;
        if (Property.TryGetProperty("emoji_id", out prop) == false)
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
    /// [ emoji_name? ] <br/>
    /// 이모지의 유니코드 문자 <br/>
    /// the unicode character of the emoji
    /// </summary>
    public string? EmojiName { get {
        JsonElement prop;
        if (Property.TryGetProperty("emoji_name", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.GetString();
    } }
}
