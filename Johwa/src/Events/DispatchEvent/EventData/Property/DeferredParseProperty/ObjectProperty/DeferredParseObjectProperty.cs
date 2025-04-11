using System.Buffers;
using System.Reflection;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseObjectMetaData : DeferredParseMetaData
{
    public override EventDataAttribute Attribute => attribute;
    public readonly DeferredParseObjectAttribute attribute;

    public readonly EventDataMetadata[] propertyMetadataArray;
    public FieldInfo fieldInfo;
    
    public DeferredParseObjectMetaData(DeferredParseObjectAttribute attribute, FieldInfo fieldInfo)
    {
        this.attribute = attribute;
        this.fieldInfo = fieldInfo;
        propertyMetadataArray = EventProperty.LoadMetadata(fieldInfo.FieldType);
    }
    public override void InitProperty(object obj, IEventData data, JsonTokenType tokenType)
    {
        if (tokenType == JsonTokenType.Null)
            return;

        if (tokenType != JsonTokenType.StartObject)
            throw new InvalidOperationException("오류");
        
        object? value = fieldInfo.GetValue(obj);
        if (value == null)
        {
            value = Activator.CreateInstance(fieldInfo.FieldType, [ new EventData(data) ])!;
            fieldInfo.SetValue(obj, value);
        }
        

        Utf8JsonReader reader = new Utf8JsonReader(new EventData(data));
        
        Span<ReadOnlyValueSet<int, EventDataMetadata>.LinkedListNode> nodeBuffer 
            = ArrayPool<ReadOnlyValueSet<int, EventDataMetadata>.LinkedListNode>.Shared.Rent(propertyMetadataArray.Length);

        ReadOnlyValueSet<int, EventDataMetadata> propertyMetadataDictionary 
            = ReadOnlyValueSet<int, EventDataMetadata>.CreateWithIHasKey(new Span<EventDataMetadata>(propertyMetadataArray), nodeBuffer);

        while (true)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.ValueSpan.Eq
                }
            }
            if ()
        }

        for (int i = 0; i < propertyMetadataArray.Length; i++)
        {
            EventDataMetadata propertyMetadata = propertyMetadataArray[i];

            if (reader.TryFindPropertyName(propertyMetadata.Attribute.name))
            {
                reader.Read();
                JsonTokenType startTokenType = reader.TokenType;
                switch (startTokenType)
                {
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                    {
                        int startIndex = (int)reader.TokenStartIndex;
                        reader.Skip();
                        int length = (int)reader.BytesConsumed - startIndex;

                        propertyMetadata.InitProperty(value, new EventData(data, startIndex, length), startTokenType);
                        continue;
                    }
                    case JsonTokenType.String:
                    case JsonTokenType.Number:
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                    case JsonTokenType.Null:
                    {
                        int startIndex = (int)reader.TokenStartIndex;
                        propertyMetadata.InitProperty(value, new EventData(data, startIndex, reader.ValueSpan.Length), startTokenType);
                        continue;
                    }

                    default:
                        continue;
                }
            }
            else
            {
                propertyMetadata.InitProperty(value, new EventData(data, -1, -1), JsonTokenType.None);
            }
        }
    }
}

public class DeferredParseObjectAttribute : DeferredParseAttribute
{
    public DeferredParseObjectAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }
    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new DeferredParseObjectMetaData(this, fieldInfo);
    }
}
public abstract class DeferredParseObjectProperty : DeferredParseProperty
{
    public DeferredParseObjectProperty(
        EventData data) : base(data) { }

    public abstract void Init();
}