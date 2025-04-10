using System.Reflection;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class EventDataPropertyAttribute : Attribute
{
    public readonly string name;
    public readonly bool isOptional;

    public EventDataPropertyAttribute(string name, bool isOptional = false)
    {
        this.name = name;
        this.isOptional = isOptional;
    }
    public abstract EventPropertyMetadata CreateMetadata(
        EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional);
}

public abstract class EventPropertyMetadata
{
    public readonly EventDataDocumentMetadata eventDataMetadata;
    public readonly FieldInfo fieldInfo;
    public readonly string name;
    public readonly bool isOptional;

    public EventPropertyMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional)
    {
        this.eventDataMetadata = eventDataMetadata;
        this.fieldInfo = fieldInfo;
        this.name = name;
        this.isOptional = isOptional;
    }
}

public static class EventProperty
{
    public static EventPropertyMetadata[] LoadMetadata(EventDataDocumentMetadata eventDataMetadata, Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<EventPropertyMetadata> propertyMetadataList = new List<EventPropertyMetadata>(fields.Length);

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            EventPropertyGroupAttribute? attribute = field.GetCustomAttribute<EventPropertyGroupAttribute>();
            if (attribute == null)
                continue;

            
            EventPropertyMetadata propertyMetadata = attribute.CreateMetadata(eventDataMetadata, field, attribute.name, attribute.isOptional);
            propertyMetadataList.Add(propertyMetadata);
        }
        return propertyMetadataList.ToArray();
    }
}