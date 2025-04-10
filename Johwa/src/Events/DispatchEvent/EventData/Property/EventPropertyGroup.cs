using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class EventPropertyGroupMetadata : EventPropertyMetadata
{
    public readonly EventPropertyMetadata[] propertyMetadataArray;
    public Type groupType;
    
    public EventPropertyGroupMetadata(Type groupType, EventPropertyGroup eventPropertyGroup, FieldInfo fieldInfo) 
        : base(eventPropertyGroup.metadata, fieldInfo, name, isOptional)
    {
        this.groupType = groupType;

        propertyMetadataArray = EventProperty.LoadMetadata();
    }

    public void SetProperty(EventPropertyGroup propertyGroup)
    {
        Utf8JsonReader reader = new Utf8JsonReader(propertyGroup.SliceData());
    }
}
public class EventPropertyGroupAttribute : EventDataPropertyAttribute
{
    public EventPropertyGroupAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventPropertyMetadata CreateMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional)
    {
        return new EventPropertyGroupMetadata(fieldInfo.FieldType,
            eventDataMetadata, fieldInfo, name, isOptional);
    }
}
public abstract class EventPropertyGroup : EventDataObjectUnit
{
    public readonly EventPropertyGroupMetadata metadata;

    protected EventPropertyGroup(EventDataDocument eventData, EventDataDocumentMetadata eventDataMetadata,
        EventPropertyGroupMetadata metadata, int startIndex, int length)
        : base(eventData, startIndex, length)
    {
        this.metadata = metadata;

        if (metadata == null){
            metadata = new EventPropertyGroupMetadata(GetType(), 
                eventDataMetadata, null, "", false
            );
        }
        metadata.SetProperty(this);
    }
}