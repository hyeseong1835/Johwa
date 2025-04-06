using System.Text.Json;
using Johwa.Core.Channel;

namespace Johwa.Gateway.Events;

[GatewayEvent("GUILD_CREATE")]
public class GuildCreateEvent : GatewayEvent
{
    public override async Task HandleAsync(DiscordGatewayClient client, JsonElement data)
    {
        string? guildId = data.GetProperty("id").GetString();
        string? guildName = data.GetProperty("name").GetString();
        string? guildIcon = data.GetProperty("icon").GetString();

        Console.WriteLine($"[ 로그 ] GUILD_CREATE: {guildId} - {guildName} - {guildIcon}");
    }
}
public struct GuildCreateEventData
{
    public readonly JsonElement jsonElement;

    public Channels Channels => new Channels();
    public int ChannelCount => jsonElement.GetProperty("channels").GetArrayLength();
}
public class GuildCreateEventAttribute : Attribute
{
    public GuildCreateEventAttribute()
    {
        
    }
}