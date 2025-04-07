using System.Text.Json;

namespace Johwa.Event.DispatchEvents;

[DispatchEvent("MESSAGE_CREATE")]
public class MessageCreateEvent : DispatchEvent
{
    public override async Task HandleAsync(DiscordGatewayClient client, JsonElement data)
    {
        string? content = data.GetProperty("content").GetString();
        string? username = data.GetProperty("author").GetProperty("username").GetString();
        string? channelId = data.GetProperty("channel_id").GetString();

        Console.WriteLine($"Message from {username} in {channelId}: \n{content}");
    }
}