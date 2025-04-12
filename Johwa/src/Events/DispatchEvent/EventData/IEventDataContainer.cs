namespace Johwa.Event.Data;

public interface IEventDataContainer : IDisposable
{
    public abstract IEnumerable<EventPropertyData> GetPropertyDataEnumerable();
    abstract void IDisposable.Dispose();
}