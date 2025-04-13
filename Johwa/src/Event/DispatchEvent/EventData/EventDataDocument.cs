namespace Johwa.Event.Data;

public class EventDataDocumentMetadata : IEventDataContainerMetadata
{
    #region 재정의

    public Type ContainerType => documentType;
    public EventPropertyGroupDescriptorAttribute[] PropertyGroupDescriptorArray => propertyGroupDescriptorArray;
    public EventPropertyDescriptorAttribute[] PropertyDescriptorArray => propertyDescriptorArray;

    #endregion

    // 필드
    public Type documentType;
    public readonly EventPropertyGroupDescriptorAttribute[] propertyGroupDescriptorArray;
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    // 생성자
    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.documentType = eventDataType;
        this.propertyGroupDescriptorArray = IEventDataContainer.LoadPropertyGroupDescriptors(eventDataType).ToArray();
        this.propertyDescriptorArray = IEventDataContainer.LoadPropertyDataDescriptors(eventDataType).ToArray();
    }
}

public abstract class EventDataDocument : IEventDataContainer
{
    #region 재정의

    Type IEventDataContainer.Type => metadata.documentType;

    ReadOnlyMemory<byte> IEventDataContainer.Data => data;
    IEventDataContainerMetadata IEventDataContainer.Metadata => metadata;
    
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
    public readonly List<EventPropertyData> propertyData;

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