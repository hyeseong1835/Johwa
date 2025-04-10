namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public readonly EventDataMetadata[] propertyMetadataArray;
    public Type eventDataType;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;

        propertyMetadataArray = EventProperty.LoadMetadata(eventDataType);
    }
}

public abstract class EventDataDocument : IEventData, IDisposable
{
    ReadOnlyMemory<byte> IEventData.Container { get {
        if (data == null)
            throw new InvalidOperationException("데이터를 참조할 수 없습니다.");
            
        return data;
    } }
    int IEventData.StartIndex => 0;
    int IEventData.Length => (data == null)? 0 : data.Length;

    byte[]? data;
    
    public EventDataDocument(byte[] data)
    {
        this.data = data;
    }
    public void Dispose()
    {
        data = null;
    }
    public abstract void Init();
    public void InitProperty(EventDataDocumentMetadata metadata)
    {
        for (int i = 0; i < metadata.propertyMetadataArray.Length; i++)
        {
            EventDataMetadata propertyMetadata = metadata.propertyMetadataArray[i];
            propertyMetadata.InitProperty(this, this);
        }
    }
}