using System.Reflection;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    #region Static

    static Dictionary<Type, EventDataDocumentMetadata> instanceDictionary = new ();
    public static EventDataDocumentMetadata GetOrCreateInstance(Type documentType)
    {
        if (instanceDictionary.TryGetValue(documentType, out EventDataDocumentMetadata? instance))
        {
            return instance;
        }
        else
        {
            EventDataDocumentMetadata newInstance = new (documentType);
            instanceDictionary.Add(documentType, newInstance);
            return newInstance;
        }
    }

    static ReadOnlyByteSpanTree<EventDataInfo> CreateDataInfoTree(Type documentType)
    {
        ReadOnlyByteSpanTree<EventDataInfo>.Builder treeBuilder = new ();

        List<EventFieldInfo> fieldDescriptorList = new();
        List<EventDataGroupInfo> dataGroupDescriptorList = new();
        int dataCount = 0;
        
        // 필드 정보
        FieldInfo[] fieldInfoArray = documentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fieldInfoArray.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfoArray[i];

            // Field
            EventFieldAttribute? propertyAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
            if (propertyAttribute != null) {
                EventFieldInfo descriptor = propertyAttribute.CreateDescriptor();
                fieldDescriptorList.Add(descriptor);
                dataCount++;

                if (descriptor.isOptional == false) {
                    minDataCount++;
                }
                continue;
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

        // 정리
        fieldDescriptorArray = fieldDescriptorList.ToArray();
        dataGroupDescriptorArray = dataGroupDescriptorList.ToArray();

        // 데이터 설명자 배열
        dataDescriptorArray = new EventDataInfo[dataCount];
        int dataDescriptorIndex = 0;
        for (int i = 0; i < fieldDescriptorArray.Length; i++)
        {
            dataDescriptorArray[dataDescriptorIndex++] = fieldDescriptorArray[i];
        }
        for (int i = 0; i < dataGroupDescriptorArray.Length; i++)
        {
            AddGroupSubDescriptors(
                dataGroupDescriptorArray[i], 
                ref dataDescriptorArray, 
                ref dataDescriptorIndex
            );
        }
        static void AddGroupSubDescriptors(EventDataGroupInfo groupDescriptor, ref EventDataInfo[] dataDescriptorArray, ref int currentIndex)
        {
            for (int i = 0; i < groupDescriptor.metadata.subFieldDescriptorArray.Length; i++)
            {
                dataDescriptorArray[currentIndex++] = groupDescriptor.metadata.subFieldDescriptorArray[i];
            }
            for (int i = 0; i < groupDescriptor.metadata.subPropertyDescriptorArray.Length; i++)
            {
                dataDescriptorArray[currentIndex++] = groupDescriptor.metadata.subPropertyDescriptorArray[i];
            }
            for (int i = 0; i < groupDescriptor.metadata.subDataGroupDescriptorArray.Length; i++)
            {
                EventDataGroupInfo subGroupDescriptor = groupDescriptor.metadata.subDataGroupDescriptorArray[i];
                AddGroupSubDescriptors(subGroupDescriptor, ref dataDescriptorArray, ref currentIndex);
            }
        }
        return treeBuilder.BuildAndDispose();
    }

    #endregion


    #region Instance

    public ReadOnlyByteSpanTree<EventDataInfo> dataInfoTree; 

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        this.dataInfoTree = CreateDataInfoTree(documentType);
    }
    #endregion
}