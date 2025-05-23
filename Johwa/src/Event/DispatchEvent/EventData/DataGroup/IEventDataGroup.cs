using System.Reflection;

namespace Johwa.Event.Data;

public interface IEventDataGroup
{
    #region Static

    public static void CreateDescriptors(Type groupType, 
        out EventDataGroupInfo[] dataGroupDescriptorArray,
        out EventFieldInfo[] fieldDescriptorArray, 
        out EventDataInfo[] dataDescriptorArray,
        out int minDataCount)
    {
        List<EventFieldInfo> fieldDescriptorList = new();
        List<EventDataGroupInfo> dataGroupDescriptorList = new();
        int dataCount = 0;
        minDataCount = 0;
        
        // 필드 정보
        FieldInfo[] fieldInfoArray = groupType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fieldInfoArray.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfoArray[i];

            // Field
            EventFieldAttribute? propertyAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
            if (propertyAttribute != null) {
                EventFieldInfo descriptor = propertyAttribute.CreateDescriptor(fieldInfo);
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
    }

    #endregion
}