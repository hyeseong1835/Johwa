using System.Reflection;
using System.Text.Json;
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
        if (tokenType == JsonTokenType.None)
            throw new InvalidOperationException("오류");
        
        object? value = fieldInfo.GetValue(obj);
        if (value == null)
        {
            value = Activator.CreateInstance(fieldInfo.FieldType, [ new EventData(data) ])!;
            fieldInfo.SetValue(obj, value);
        }

        for (int i = 0; i < propertyMetadataArray.Length; i++)
        {
            EventDataMetadata propertyMetadata = propertyMetadataArray[i];
            Utf8JsonReader reader = new Utf8JsonReader(new EventData(data));

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