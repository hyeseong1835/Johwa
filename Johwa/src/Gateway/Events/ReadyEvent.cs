using System.Text.Json;

namespace Johwa.Gateway.Events;

[GatewayEvent("READY")]
public class ReadyEvent : GatewayEvent
{
    public override async Task HandleAsync(DiscordGatewayClient client, JsonElement data)
    {
        Console.WriteLine("[ 로그 ] READY");
    }
}