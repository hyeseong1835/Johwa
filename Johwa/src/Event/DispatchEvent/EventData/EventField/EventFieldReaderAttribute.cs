namespace Johwa.Event.Data;

public class EventFieldReaderAttribute : Attribute
{
    // 필드 & 프로퍼티
    public Type fieldType;

    // 생성자
    public EventFieldReaderAttribute(Type fieldType)
    {
        this.fieldType = fieldType;
    }
}