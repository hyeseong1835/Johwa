using System.Text.Json;

namespace Johwa.Event.DispatchEvents;

[DispatchEvent("GUILD_CREATE")]
public class GuildCreateEvent : DispatchEvent
{
    public override async Task HandleAsync(DiscordGatewayClient client, JsonElement data)
    {
        GuildCreateEventData guildCreateEventData = new GuildCreateEventData(data);
    }
}