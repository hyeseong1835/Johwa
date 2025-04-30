namespace Johwa.Event;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DispatchEventAttribute : Attribute
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