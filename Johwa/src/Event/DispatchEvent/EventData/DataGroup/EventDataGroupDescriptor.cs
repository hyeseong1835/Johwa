using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupDescriptor
{
    public FieldInfo fieldInfo;

    public EventDataGroupDescriptor(FieldInfo fieldInfo)
    {
        this.fieldInfo = fieldInfo;
    }
}