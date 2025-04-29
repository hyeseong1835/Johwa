using Johwa.Common.Debug;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public interface IEventField
{
    #region Object
    
    public struct CreateData
    {
        public EventFieldInfo info;
        public nint fieldPtr;
        public ReadOnlyMemory<byte> data;
        public JsonTokenType tokenType;

        public CreateData(EventFieldInfo descriptor, nint fieldPtr,
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            this.info = descriptor;
            this.fieldPtr = fieldPtr;
            this.data = data;
            this.tokenType = tokenType;
        }
    }
    
    #endregion


    #region Static

    public static void ReadDocumentField(FieldInfo fieldInfo, ref ReadOnlyByteSpanTree<EventDataInfo>.Builder treeBuilder)
    {
        // Field
        Type fieldType = fieldInfo.FieldType;

        EventFieldAttribute? fieldAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
        if (fieldAttribute == null) 
            return;

        

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


    #region Instance



    #endregion
}