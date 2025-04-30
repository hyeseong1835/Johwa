using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class EventFieldInfo : EventDataInfo
{
    #region Instance

    public FieldInfo fieldInfo;

    public EventFieldInfo(FieldInfo fieldInfo, string name, bool isOptional, bool isNullable)
        : base(name, isOptional, isNullable)
    {
        this.fieldInfo = fieldInfo;
    }
    
    unsafe public override void ReadData<TDocument>(TDocument* documentPtr, ReadOnlyMemory<byte> jsonData, JsonTokenType jsonTokenType)
    {
        // Field
        EventFieldAttribute? fieldAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
        if (fieldAttribute == null) 
            return;

        Type fieldType = fieldInfo.FieldType;

        EventFieldReader.ReadField(createData);

        ReadOnlySpan<byte> fieldNameByteSpan = eventFieldInfo.name.AsByteSpan();

        treeBuilder.Add(fieldNameByteSpan, eventFieldInfo);

        EventFieldInfo descriptor = fieldAttribute.CreateDescriptor();
        fieldDescriptorList.Add(descriptor);
        dataCount++;

        if (descriptor.isOptional == false) {
            minDataCount++;
        }

        // DataGroup
        EventDataGroupAttribute? propertyGroupAttribute = fieldInfo.GetCustomAttribute<EventDataGroupAttribute>();
        if (propertyGroupAttribute != null) {
            EventDataGroupInfo descriptor = propertyGroupAttribute.GetDescriptor(fieldInfo);
            dataGroupDescriptorList.Add(descriptor);

            dataCount += descriptor.metadata.dataDescriptorCount;
            minDataCount += descriptor.metadata.minDataCount;
            continue;
        }
    }

    #endregion
}