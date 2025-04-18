using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupAttribute : Attribute
{
    public EventDataGroupDescriptor CreateDescriptor(FieldInfo fieldInfo)
    {
        EventDataGroupMetadata metadata = EventDataGroupMetadata.GetInstance(fieldInfo.FieldType);
        
        return new EventDataGroupDescriptor(fieldInfo, metadata);
    }
}