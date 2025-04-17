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
                EventPropertyGroupMetadata metadata = EventPropertyGroupMetadata.GetInstance(groupType);
                result.AddRange(metadata.propertyDescriptorArray);
                continue;
            }
        }
        this.propertyDescriptorArray = result.ToArray();
    }
}

public abstract class EventDataDocument : IEventDataGroup, IDisposable
{
    #region 재정의

    // IDisposable
    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        for (int i = 0; i < fieldSet.Count; i++)
        {
            EventField field = fieldSet[i];
            field.Dispose();
        }
        for (int i = 0; i < propertySet.Count; i++)
        {
            EventProperty property = propertySet[i];
            property.Dispose();
        }
    }
    
    #endregion


    #region 필드

    public readonly EventDataDocumentMetadata metadata;
    public ReadOnlyMemory<byte> data;
    public readonly EventField[] fieldSet;
    public readonly List<EventProperty> propertySet;

    #endregion


    #region 생성자

    public EventDataDocument(ReadOnlyMemory<byte> data)
    {
        this.metadata = GetMetadata();
        this.data = data;
        this.fieldSet = IEventDataGroup.CreateFieldSet(metadata.documentType, data, metadata.propertyDescriptorArray);
        this.propertySet = IEventDataGroup.CreatePropertySet(metadata.documentType, data, metadata.propertyDescriptorArray);
    }

    #endregion
    

    #region 추상
    
    protected abstract EventDataDocumentMetadata GetMetadata();

    #endregion


    #region 메서드

    

    #endregion
}