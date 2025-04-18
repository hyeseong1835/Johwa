using System.Text.Json;

namespace Johwa.Event.Data;

public class EventProperty : IDisposable
{
    #region Static


    #endregion


    #region Instance

    // 재정의 (IDisposable)
    public virtual void Dispose() { }

    // 생성자
    public EventProperty(object declaringObject,
            EventPropertyDescriptor descriptor, 
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        
    }


    #endregion
}