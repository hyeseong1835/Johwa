using System.Reflection;

namespace Johwa.Event.Data;

public interface IEventDataGroupMetadata
{
    #region Static

    public static EventPropertyAttribute[] LoadPropertyDescriptors(Type groupType)
    {
        List<EventPropertyAttribute> result = new();

        // 필드 정보 가져오기
        FieldInfo[] fields = groupType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyDescriptorAttribute 로드
            EventPropertyAttribute? propertyDescriptor = field.GetCustomAttribute<EventPropertyAttribute>();
            if (propertyDescriptor != null) {
                propertyDescriptor.fieldInfo = field;
                result.Add(propertyDescriptor);
                continue;
            }

            // EventPropertyGroupDescriptorAttribute 로드
            EventPropertyGroupAttribute? propertyGroupDescriptor = field.GetCustomAttribute<EventPropertyGroupAttribute>();
            if (propertyGroupDescriptor != null) {
                // 메타데이터 로드
                EventPropertyGroupMetadata metadata = EventPropertyGroupMetadata.GetInstance(groupType);
                result.AddRange(metadata.propertyDescriptorArray);
                continue;
            }
        }
        return result.ToArray();
    }

    #endregion

    public EventPropertyAttribute[] PropertyDescriptorArray { get; }
}
public interface IEventDataGroup : IDisposable
{
    public IEventDataGroupMetadata GroupMetadata { get; }
}