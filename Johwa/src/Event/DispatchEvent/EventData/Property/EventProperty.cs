using System.Text.Json;

namespace Johwa.Event.Data;

public class EventProperty : IDisposable
{
    #region Object

    public struct CreateData
    {
        public object declaringObject;
        public EventPropertyDescriptor descriptor;
        public ReadOnlyMemory<byte> data;
        public JsonTokenType tokenType;
        
        public CreateData(object declaringObject,
            EventPropertyDescriptor descriptor, 
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            this.declaringObject = declaringObject;
            this.descriptor = descriptor;
            this.data = data;
            this.tokenType = tokenType;
        }
    }

    #endregion


    #region Static


    #endregion


    #region Instance

    // 재정의 (IDisposable)
    public virtual void Dispose() { }

    // 생성자
    public EventProperty(CreateData createData)
    {
        // 필드 타입이 아니면 예외
        if (!createData.descriptor.isFieldTypeEventProperty) {
            throw new InvalidOperationException($"EventProperty는 필드 타입이 아닙니다. : {createData.descriptor.name}");
        }
    }

    #endregion
}