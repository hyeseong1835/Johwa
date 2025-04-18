namespace Johwa.Event.Data;

public class EventPropertySet
{
    // 필드
    readonly List<EventProperty> list;

    // 생성자
    public EventPropertySet(EventPropertyDescriptor[] descriptors)
    {
        list = new List<EventProperty>(descriptors.Length);
    }

    public void Add(EventProperty property)
    {
        list.Add(property);
    }
}