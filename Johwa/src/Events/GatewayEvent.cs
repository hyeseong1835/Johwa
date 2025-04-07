using System.Text.Json;

namespace Johwa.Event;

public abstract class DispatchEvent
{
    public abstract Task HandleAsync(DiscordGatewayClient client, JsonElement data);
}