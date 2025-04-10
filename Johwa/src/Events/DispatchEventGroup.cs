using Johwa.Event.Data;

namespace Johwa.Event;

public class DispatchEventGroup
{
    readonly List<DispatchEvent> events;
    readonly Func<EventDataDocument> createEventData;

    public DispatchEventGroup(List<DispatchEvent> events, Func<EventDataDocument> createEventData)
    {
        this.events = events;
        this.createEventData = createEventData;
    }
    public void OnHandled(DiscordGatewayClient client, ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < events.Count; i++)
        {
            DispatchEvent dispatchEvent = events[i];
            if (dispatchEvent.isEnabled) {
                using (EventDataDocument eventData = createEventData.Invoke())
                {
                    eventData.Init();
                    dispatchEvent.Handle(client, data);

                    for (i = i + 1; i < events.Count; i++)
                    {
                        dispatchEvent = events[i];
                        if (dispatchEvent.isEnabled) {
                            dispatchEvent.Handle(client, data);
                            break;
                        }
                    }
                }
                return;
            }
        }
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