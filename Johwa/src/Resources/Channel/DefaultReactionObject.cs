using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.Channel;

/// <summary>
/// 포럼 게시글에 대한 기본 반응 이모지를 지정하는 객체입니다. <br/>
/// An object that specifies the emoji to use as the default way to react to a forum post.
/// </summary>
public struct DefaultReactionObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public DefaultReactionObject(JsonElement defaultReactionProperty)
    {
        this.Property = defaultReactionProperty;
    }

    /// <summary>
    /// [ emoji_id? ] <br/>
    /// 길드의 커스텀 이모지 ID <br/>
    /// the id of a guild's custom emoji
    /// </summary>
    public ulong? EmojiId
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("emoji_id", out prop) == false)
                return null;

            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            if (prop.ValueKind == JsonValueKind.String)
            {
                if (ulong.TryParse(prop.GetString(), out var id))
                    return id;
            }

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetUInt64();

            return null;
        }
    }

    /// <summary>
    /// [ emoji_name? ] <br/>
    /// 유니코드 이모지 문자열 <br/>
    /// the unicode character of the emoji
    /// </summary>
    public string? EmojiName
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("emoji_name", out prop) == false)
                return null;

            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            return prop.GetString();
        }
    }
}
