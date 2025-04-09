using System.Text.Json;

namespace Johwa.Event.DispatchEvents;

[DispatchEvent(DispatchEventType.GUILD_CREATE)]
public class GuildCreateEvent : DispatchEvent
{
    public override void Handle(DiscordGatewayClient client, JsonElement data)
    {
        GuildCreateEventData guildCreateEventData = new GuildCreateEventData(data);

        Console.WriteLine($"[ 로그 ] GUILD_CREATE: {guildCreateEventData.Guild?.Name}\n{string.Join(", ", guildCreateEventData.Channels.ToEnumerable().Select(x => x.Name))}");
    }
}