namespace Johwa.Event.Data;

public class EventFieldSet
{
    // 필드
    readonly List<EventField> list;

    // 생성자
    public EventFieldSet(EventFieldInfo[] descriptors)
    {
        list = new List<EventField>(descriptors.Length);
    }

    public void Add(EventField field)
    {
        list.Add(field);
    }
}