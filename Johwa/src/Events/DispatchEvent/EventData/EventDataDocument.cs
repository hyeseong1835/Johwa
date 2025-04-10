using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public readonly EventPropertyMetadata[] propertyMetadataArray;
    public Type eventDataType;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;

        // propertyMetadataArray
        FieldInfo[] fields = eventDataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<EventPropertyMetadata> propertyMetadataList = new List<EventPropertyMetadata>(fields.Length);

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            EventDataPropertyAttribute? attribute = field.GetCustomAttribute<EventDataPropertyAttribute>();
            if (attribute == null)
                continue;

            
            EventPropertyMetadata propertyMetadata = attribute.CreateMetadata(this, field, attribute.name, attribute.isOptional);
            propertyMetadataList.Add(propertyMetadata);
        }
        propertyMetadataArray = propertyMetadataList.ToArray();
    }

    public void SetProperty(EventDataDocument eventData)
    {
        /*
        Utf8JsonReader reader = new Utf8JsonReader(data);
        
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;
            
            string? propName = reader.GetString();
            if (propName == null) continue;

            reader.Read(); 

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    
                    break;
                case JsonTokenType.StartArray:

                    break;
                case JsonTokenType.String:
                {
                    switch (propName)
                    {
                        case "joined_at":
                            joinedAt = new DateTimeReader(reader.ValueSpan);
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported property name: {propName}");
                    }
                    break;
                }
                case JsonTokenType.Number:
                {
                    break;
                }
                case JsonTokenType.True:
                {
                    switch (propName)
                    {
                        case "large":
                            isLarge = true;
                            break;
                        case "unavailable":
                            isUnavailable = true;
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported property name: {propName}");
                    }
                    break;
                }
                case JsonTokenType.False:
                {
                    switch (propName)
                    {
                        case "large":
                            isLarge = false;
                            break;
                        case "unavailable":
                            isUnavailable = false;
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported property name: {propName}");
                    }
                    break;
                }
                default:
                    throw new NotSupportedException($"Unsupported token type: {reader.TokenType}");
            }
            if (propName == "joined_at")
            {
                joinedAt = new DateTimeReader(reader.ReadSpan());
            }
            else if (propName == "large")
            {
                data.SetProperty("large", reader.ReadBoolean());
            }
            else if (propName == "unavailable")
            {
                data.SetProperty("unavailable", reader.ReadBoolean());
            }
        }
        */
    }
}

public abstract class EventDataDocument : IEventDataObject, IDisposable
{
    byte[]? data;
    ReadOnlySpan<byte> IEventDataObject.Data { get {
        if (data == null)
            throw new InvalidOperationException("데이터를 참조할 수 없습니다.");
            
        return data;
    } }
    
    public EventDataDocument(byte[] data)
    {
        this.data = data;
    }
    public void Dispose()
    {
        data = null;
    }
    public abstract void Init();
}