namespace Johwa.Event.Data;

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