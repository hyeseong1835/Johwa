using System.Text.Json;

namespace Johwa.Event.DispatchEvents;

[DispatchEvent(DispatchEventType.MESSAGE_CREATE)]
public class MessageCreateEvent : DispatchEvent
{
    public override async Task Handle(DiscordGatewayClient client, JsonElement data)
    {
        //MessageCreateEventData messageCreateEventData = new MessageCreateEventData(data);

        //Console.WriteLine($"Message from {username} in {channelId}: \n{content}");
    }
}