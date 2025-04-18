using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupDescriptor
{
    public EventDataGroupMetadata metadata;
    public FieldInfo fieldInfo;

    public EventDataGroupDescriptor(FieldInfo fieldInfo, EventDataGroupMetadata metadata)
    {
        this.fieldInfo = fieldInfo;
        this.metadata = metadata;
    }
}