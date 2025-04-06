using Johwa.Rest;
using Johwa.Gateway;

namespace Johwa.Core;

public class DiscordBot
{
    public readonly string token;

    public DiscordGatewayClient gatewayClient;
    public DiscordRestClient restClient;

    public DiscordBot(string token)
    {
        this.token = token;
        gatewayClient = new DiscordGatewayClient(this);
        restClient = new DiscordRestClient(this);
    }
    public DiscordBot(string token, 
        DiscordGatewayClient gatewayClient, DiscordRestClient restClient)
    {
        this.token = token;
        this.gatewayClient = gatewayClient;
        this.restClient = restClient;
    }

    public async Task Connect()
    {
        await gatewayClient.Connect();
    }
}