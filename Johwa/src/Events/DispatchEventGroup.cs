using System.Text.Json;

namespace Johwa.Event;

public class DispatchEventGroup
{
    readonly List<DispatchEvent> events;

    public DispatchEventGroup()
    {
        events = new List<DispatchEvent>();
    }
    public DispatchEventGroup(List<DispatchEvent> events)
    {
        this.events = events;
    }
    public void Execute(DiscordGatewayClient client, ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < events.Count; i++)
        {
            DispatchEvent dispatchEvent = events[i];
            dispatchEvent.Handle(client, data);
        }
    }
}