namespace Johwa.Event;

public abstract class DispatchEvent
{
    public bool isEnabled;
    public abstract void Handle(DiscordGatewayClient client, ReadOnlyMemory<byte> dataMemory);
}