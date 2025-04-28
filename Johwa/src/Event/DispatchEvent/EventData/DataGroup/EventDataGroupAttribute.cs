using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupAttribute : Attribute
{
    public EventDataGroupInfo GetDescriptor(FieldInfo fieldInfo)
    {
        return new EventDataGroupInfo(fieldInfo);
    }
}