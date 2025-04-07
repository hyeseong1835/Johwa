using System.Text.Json;

namespace Johwa.Event.Events;

[DispatchEvent("READY")]
public class ReadyEvent : DispatchEvent
{
    public override async Task HandleAsync(DiscordGatewayClient client, JsonElement data)
    {
        Console.WriteLine("[ 로그 ] READY");
    }
}