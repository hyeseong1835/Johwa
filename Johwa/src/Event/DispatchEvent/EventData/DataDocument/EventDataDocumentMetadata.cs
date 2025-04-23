using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    // 필드
    public Type documentType;
    public readonly EventFieldDescriptor[] propertyDescriptorArray;

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        this.documentType = documentType;
        List<EventFieldDescriptor> result = new();

        // 필드 정보 가져오기
        FieldInfo[] fields = documentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyDescriptorAttribute 로드
            EventFieldAttribute? propertyAttribute = field.GetCustomAttribute<EventFieldAttribute>();
            if (propertyAttribute != null) {
                EventFieldDescriptor descriptor = propertyAttribute.CreateDescriptor(group, field, field.FieldType.IsSubclassOf(typeof(EventField)));
                result.Add(descriptor);
                continue;
            }

            // EventPropertyGroupDescriptorAttribute 로드
            EventDataGroupAttribute? propertyGroupAttribute = field.GetCustomAttribute<EventDataGroupAttribute>();
            if (propertyGroupAttribute != null) {
                // 메타데이터 로드
                EventDataGroupMetadata metadata = EventDataGroupMetadata.GetInstance(groupType);
                result.AddRange(metadata.subFieldDescriptorArray);
                continue;
            }
        }
        this.propertyDescriptorArray = result.ToArray();
    }
}
