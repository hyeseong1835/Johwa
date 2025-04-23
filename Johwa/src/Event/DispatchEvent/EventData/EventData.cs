using System.Text.Json;

namespace Johwa.Event.Data;

public abstract class EventData : IDisposable
{
    #region Object

    public struct EventDataCreateData
    {
        public EventDataDescriptor descriptor;
        public IEventDataGroup declaringGroup;
        public ReadOnlyMemory<byte> data;
        public JsonTokenType tokenType;
        
        public EventDataCreateData(EventDataDescriptor descriptor, IEventDataGroup declaringGroup,
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            this.declaringGroup = declaringGroup;
            this.data = data;
            this.tokenType = tokenType;
        }
    }

    #endregion


    #region Instance

    // 재정의 (IDisposable)
    public virtual void Dispose() { }

    #endregion
}

