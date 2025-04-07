namespace Johwa.Event;

[AttributeUsage(AttributeTargets.Class)]
public class DispatchEventAttribute : Attribute
{
    public readonly string eventName;

    public DispatchEventAttribute(string eventName)
    {
        this.eventName = eventName;
    }
}