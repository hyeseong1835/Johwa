using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class EventDataAttribute : Attribute
{
    public readonly string name;
    public readonly bool isOptional;

    public EventDataAttribute(string name, bool isOptional = false)
    {
        this.name = name;
        this.isOptional = isOptional;
    }
    public abstract EventDataMetadata CreateMetadata(FieldInfo fieldInfo);
}

public abstract class EventDataMetadata
{
    public abstract EventDataAttribute Attribute { get; }
    public readonly FieldInfo fieldInfo;

    public EventDataMetadata(FieldInfo fieldInfo)
    {
        this.fieldInfo = fieldInfo;
    }

    public abstract void InitProperty(object obj, ReadOnlyMemory<byte> container, JsonTokenType tokenType);
}

public abstract class EventProperty
{
    public ReadOnlyMemory<byte> data;

    public EventProperty(ReadOnlyMemory<byte> data)
    {
        this.data = data;
    }

    public static EventDataMetadata[] LoadMetadata(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<EventDataMetadata> propertyMetadataList = new List<EventDataMetadata>(fields.Length);

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            EventDataAttribute? attribute = field.GetCustomAttribute<EventDataAttribute>();
            if (attribute == null)
                continue;
            
            EventDataMetadata propertyMetadata = attribute.CreateMetadata(field);
            propertyMetadataList.Add(propertyMetadata);
        }
        return propertyMetadataList.ToArray();
    }
}