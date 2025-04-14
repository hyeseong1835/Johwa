namespace Johwa.Event.Data;

public class EventDataDocumentMetadata : IEventDataContainerMetadata
{
    #region 재정의

    Type IEventDataGroupMetadata.GroupType => documentType;
    public EventPropertyDescriptorAttribute[] PropertyDescriptorArray => propertyDescriptorArray;

    #endregion

    // 필드
    public Type documentType;
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        this.documentType = documentType;
        this.propertyDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(documentType);
    }
}

public abstract class EventDataDocument : IEventDataContainer
{
    #region 재정의

    // IEventDataContainer
    ReadOnlyMemory<byte> IEventDataContainer.Data => data;
    IEventDataContainerMetadata IEventDataContainer.ContainerMetadata => metadata;
    
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