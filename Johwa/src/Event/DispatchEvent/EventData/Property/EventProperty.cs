using System.Text.Json;

namespace Johwa.Event.Data;

public class EventProperty : EventData
{
    #region Static


    #endregion


    #region Instance

    // 필드
    public EventPropertyDescriptor descriptor;

    // 생성자
    public EventProperty(object declaringObject,
            EventPropertyDescriptor descriptor, 
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        this.descriptor = descriptor;
    }


    #endregion
}