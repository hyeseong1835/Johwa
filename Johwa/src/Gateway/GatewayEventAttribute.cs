
namespace Johwa.Gateway;

[AttributeUsage(AttributeTargets.Class)]
public class GatewayEventAttribute : Attribute
{
    public string eventName { get; }

    public GatewayEventAttribute(string eventName)
    {
        this.eventName = eventName;
    }
}