using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupAttribute : Attribute
{
    public EventDataGroupDescriptor GetDescriptor(FieldInfo fieldInfo)
    {
        return new EventDataGroupDescriptor(fieldInfo);
    }
}