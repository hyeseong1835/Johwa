using System.Text.Json;

namespace Johwa.Core.Channel;

/// <summary>
/// Discord의 채널(Channel) 객체를 표현하는 구조체입니다.
/// </summary>
public struct DirectMessageChannel : IChannel
{
    JsonElement IChannel.Data => data;
    public readonly JsonElement data;

    /// <summary> 
    /// [ GUILD_TEXT, DM, GUILD_VOICE, GROUP_DM, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 이름 
    /// </summary>
    public string? Name => data.TryGetProperty("name", out var v) ? v.GetString() : null;

    /// <summary> 마지막 메시지 ID </summary>
    public ulong? LastMessageId => data.TryGetProperty("last_message_id", out var v) ? v.GetUInt64() : null;

    

    public DirectMessageChannel(JsonElement data)
    {
        this.data = data;
    }
}
