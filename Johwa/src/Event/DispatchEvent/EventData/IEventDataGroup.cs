using System.Reflection;

namespace Johwa.Event.Data;

public interface IEventDataGroup
{
    #region Static

    public static void CreateDescriptors(Type groupType, 
        out EventFieldDescriptor[] fieldDescriptorArray, 
        out EventPropertyDescriptor[] propertyDescriptorArray, 
        out EventDataGroupDescriptor[] dataGroupDescriptorArray,
        out EventDataDescriptor[] dataDescriptorArray,
        out int minDataCount)
    {
        List<EventFieldDescriptor> fieldDescriptorList = new();
        List<EventPropertyDescriptor> propertyDescriptorList = new();
        List<EventDataGroupDescriptor> dataGroupDescriptorList = new();
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
                EventFieldDescriptor descriptor = propertyAttribute.CreateDescriptor(fieldInfo);
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
                EventDataGroupDescriptor descriptor = propertyGroupAttribute.GetDescriptor(fieldInfo);
                dataGroupDescriptorList.Add(descriptor);

                dataCount += descriptor.metadata.dataDescriptorCount;
                minDataCount += descriptor.metadata.minDataCount;
                continue;
            }
        }

        // 프로퍼티 정보
        PropertyInfo[] propertyInfoArray = groupType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < propertyInfoArray.Length; i++)
        {
            PropertyInfo propertyInfo = propertyInfoArray[i];

            // Property
            EventPropertyAttribute? propertyAttribute = propertyInfo.GetCustomAttribute<EventPropertyAttribute>();
            if (propertyAttribute != null) {
                EventPropertyDescriptor descriptor = propertyAttribute.CreateDescriptor(propertyInfo);
                propertyDescriptorList.Add(descriptor);
                dataCount++;

                if (descriptor.isOptional == false) {
                    minDataCount++;
                }
                continue;
            }
        }

        // 정리
        fieldDescriptorArray = fieldDescriptorList.ToArray();
        dataGroupDescriptorArray = dataGroupDescriptorList.ToArray();
        propertyDescriptorArray = propertyDescriptorList.ToArray();

        // 데이터 설명자 배열
        dataDescriptorArray = new EventDataDescriptor[dataCount];
        int dataDescriptorIndex = 0;
        for (int i = 0; i < fieldDescriptorArray.Length; i++)
        {
            dataDescriptorArray[dataDescriptorIndex++] = fieldDescriptorArray[i];
        }
        for (int i = 0; i < propertyDescriptorArray.Length; i++)
        {
            dataDescriptorArray[dataDescriptorIndex++] = propertyDescriptorArray[i];
        }
        for (int i = 0; i < dataGroupDescriptorArray.Length; i++)
        {
            AddGroupSubDescriptors(
                dataGroupDescriptorArray[i], 
                ref dataDescriptorArray, 
                ref dataDescriptorIndex
            );
        }
        static void AddGroupSubDescriptors(EventDataGroupDescriptor groupDescriptor, ref EventDataDescriptor[] dataDescriptorArray, ref int currentIndex)
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
                EventDataGroupDescriptor subGroupDescriptor = groupDescriptor.metadata.subDataGroupDescriptorArray[i];
                AddGroupSubDescriptors(subGroupDescriptor, ref dataDescriptorArray, ref currentIndex);
            }
        }
    }

    #endregion
}