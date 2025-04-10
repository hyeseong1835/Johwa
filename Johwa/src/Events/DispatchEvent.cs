using System.Text.Json;

namespace Johwa.Event;

public abstract class DispatchEvent
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    protected class DispatchEventAttribute : Attribute
    {
        public readonly string eventName;

        public DispatchEventAttribute(string eventName)
        {
            this.eventName = eventName;
        }
        public DispatchEventAttribute(DispatchEventType eventType)
        {
            this.eventName = eventType.ToString();
        }
    }

    public bool isEnabled;
    public abstract void Handle(DiscordGatewayClient client, ReadOnlySpan<byte> data);
}