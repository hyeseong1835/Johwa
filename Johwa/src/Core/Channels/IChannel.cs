using System.Text.Json;

namespace Johwa.Core.Channel;

public interface IChannel
{
    public JsonElement Data { get; }

    /// <summary> 
    /// [ 모두 ] <br/>
    /// 채널 ID (Snowflake) 
    /// </summary>
    public ulong Id => Data.GetProperty("id").GetUInt64();

    /// <summary> 
    /// [ 모두 ] <br/>
    /// 채널 유형
    /// </summary>
    public ChannelType Type => (ChannelType)Data.GetProperty("type").GetInt32();

    /// <summary> 
    /// [ GUILD_TEXT, DM, GUILD_VOICE, GROUP_DM, GUILD_ANNOUNCEMENT, ANNOUNCEMENT_THREAD, PUBLIC_THREAD, PRIVATE_THREAD, GUILD_STAGE_VOICE, GUILD_DIRECTORY, GUILD_FORUM, GUILD_MEDIA ] <br/>
    /// 채널 이름 
    /// </summary>
    public string? Name => Data.TryGetProperty("name", out var v) ? v.GetString() : null;
}
