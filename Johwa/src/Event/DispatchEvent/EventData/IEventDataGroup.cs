using System.Text.Json;
using System.Reflection;
using Johwa.Common.Debug;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public interface IEventDataGroup
{
    #region Static

    public static void CreateDescriptors(Type groupType, out EventFieldDescriptor[] fieldDescriptors, out EventPropertyDescriptor[] propertyDescriptors, out EventDataGroupDescriptor[] dataGroupDescriptors)
    {
        List<EventFieldDescriptor> fieldDescriptorList = new();
        List<EventPropertyDescriptor> propertyDescriptorList = new();
        List<EventDataGroupDescriptor> dataGroupDescriptorList = new();
        
        // 필드 정보
        FieldInfo[] fieldInfoArray = groupType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fieldInfoArray.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfoArray[i];

            // Field
            EventFieldAttribute? propertyAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
            if (propertyAttribute != null) {
                EventFieldDescriptor descriptor = propertyAttribute.CreateDescriptor(fieldInfo, fieldInfo.FieldType.IsSubclassOf(typeof(EventField)));
                fieldDescriptorList.Add(descriptor);
                continue;
            }

            // DataGroup
            EventDataGroupAttribute? propertyGroupAttribute = fieldInfo.GetCustomAttribute<EventDataGroupAttribute>();
            if (propertyGroupAttribute != null) {
                EventDataGroupDescriptor descriptor = propertyGroupAttribute.CreateDescriptor(fieldInfo);
                dataGroupDescriptorList.Add(descriptor);
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
                continue;
            }
        }

        // 정리
        fieldDescriptors = fieldDescriptorList.ToArray();
        dataGroupDescriptors = dataGroupDescriptorList.ToArray();
        propertyDescriptors = propertyDescriptorList.ToArray();
    }

    
    #endregion


    #region Instance

    public int FieldDescriptorCount { get; }
    public int PropertyDescriptorCount { get; }

    #endregion
}