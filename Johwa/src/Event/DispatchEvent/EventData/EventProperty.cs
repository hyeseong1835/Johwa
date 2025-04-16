using System.Text.Json;

namespace Johwa.Event.Data;

public struct EventPropertyCreateData
{
    public IEventDataContainer container;
    public EventPropertyDescriptor descriptor;
    public ReadOnlyMemory<byte> data;
    public JsonTokenType tokenType;
    
    public EventPropertyCreateData(IEventDataContainer container,
        EventPropertyDescriptor descriptor, 
        ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        this.container = container;
        this.descriptor = descriptor;
        this.data = data;
        this.tokenType = tokenType;
    }
}
public abstract class EventProperty : IDisposable
{
    public class Info
    {
        public EventPropertyDescriptor descriptor;
        public ReadOnlyMemory<byte> data;

        public Info(EventPropertyDescriptor descriptor, ReadOnlyMemory<byte> data)
        {
            this.descriptor = descriptor;
            this.data = data;
        }
    }
    #region Instance

    void IDisposable.Dispose()
    {
        if (info != null)
        {
            info.data = default;
            info = null;
        }
    }

    public Info? info;

    public virtual void Init(Info info)
    {
        this.info = info;
    }

    #endregion
}