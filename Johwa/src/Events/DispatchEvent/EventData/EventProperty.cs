using System.Reflection;
using System.Text.Json;
using Johwa.Common;

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

public abstract class EventDataMetadata : IHasKey<string>
{
    string IHasKey<string>.Key => Attribute.name;
    
    public abstract EventDataAttribute Attribute { get; }

    public abstract void InitProperty(object obj, IEventData container, JsonTokenType tokenType);
}

public abstract class EventProperty : IEventData
{
    ReadOnlyMemory<byte> IEventData.Container => data.container;
    int IEventData.StartIndex => data.startIndex;
    int IEventData.Length => data.length;

    public EventData data;

    public EventProperty(EventData data)
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