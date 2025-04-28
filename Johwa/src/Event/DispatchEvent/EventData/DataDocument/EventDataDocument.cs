namespace Johwa.Event.Data;

public abstract class EventDataDocument : EventDataContainer
{
    #region 재정의

    public override EventDataContainerMetadata ContainerMetadata => metadata;
    
    #endregion


    #region 필드

    public readonly EventDataDocumentMetadata metadata;

    #endregion


    #region 생성자

    public EventDataDocument(EventDataDocumentMetadata metadata, ReadOnlyMemory<byte> data) : base (data)
    {
        this.data = data;
        this.metadata = metadata;
    }

    #endregion


    #region 메서드

    

    #endregion
}