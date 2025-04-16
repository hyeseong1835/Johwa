using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    // 필드
    public Type documentType;
    public readonly EventPropertyDescriptor[] propertyDescriptorArray;

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        this.documentType = documentType;
        List<EventPropertyDescriptor> result = new();

        // 필드 정보 가져오기
        FieldInfo[] fields = documentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyDescriptorAttribute 로드
            EventPropertyAttribute? propertyAttribute = field.GetCustomAttribute<EventPropertyAttribute>();
            if (propertyAttribute != null) {
                EventPropertyDescriptor descriptor = propertyAttribute.CreateDescriptor(group, field, field.FieldType.IsSubclassOf(typeof(EventProperty)));
                result.Add(descriptor);
                continue;
            }

            // EventPropertyGroupDescriptorAttribute 로드
            EventPropertyGroupAttribute? propertyGroupAttribute = field.GetCustomAttribute<EventPropertyGroupAttribute>();
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

public abstract class EventDataDocument : IEventDataContainer
{
    #region 재정의

    // IEventDataContainer
    ReadOnlyMemory<byte> IEventDataContainer.Data => data;
    EventPropertyDescriptor[] IEventDataGroup.PropertyDescriptorArray => metadata.propertyDescriptorArray;
    
    // IDisposable
    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        foreach (IDisposable property in propertyData)
        {
            property.Dispose();
        }
    }
    
    #endregion


    #region 필드

    public readonly EventDataDocumentMetadata metadata;
    public ReadOnlyMemory<byte> data;
    public readonly List<EventProperty> propertyData;

    #endregion


    #region 생성자

    public EventDataDocument(ReadOnlyMemory<byte> data)
    {
        this.metadata = GetMetadata();
        this.data = data;
        this.propertyData = ((IEventDataContainer)this).CreateProperties();
    }

    #endregion
    

    #region 추상
    
    protected abstract EventDataDocumentMetadata GetMetadata();

    #endregion


    #region 메서드

    

    #endregion
}