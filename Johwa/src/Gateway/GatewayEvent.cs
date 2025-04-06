using System.Text.Json;

namespace Johwa.Gateway;

public abstract class GatewayEvent
{
    public abstract Task HandleAsync(DiscordGatewayClient client, JsonElement data);
}