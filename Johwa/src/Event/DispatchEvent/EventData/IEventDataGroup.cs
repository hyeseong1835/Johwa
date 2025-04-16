namespace Johwa.Event.Data;

public interface IEventDataGroup : IDisposable
{
    public EventPropertyDescriptor[] PropertyDescriptorArray { get; }
}