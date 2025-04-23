namespace Johwa.Event.Data;

public abstract class EventDataGroup : IEventDataGroup
{
    #region Object

    public struct CreateData
    {
        public EventDataGroupDescriptor descriptor;

        public CreateData(EventDataGroupDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }
    }

    #endregion

    // 필드
    public EventDataGroupMetadata metadata;
    public EventDataGroupDescriptor descriptor;
    public readonly EventFieldSet subFieldSet;
    public readonly EventPropertySet subPropertySet;
    public readonly EventDataGroupSet subGroupSet;

    // 생성자
    public EventDataGroup(EventDataGroupMetadata metadata, CreateData createData)
    {
        this.metadata = metadata;
        this.descriptor = createData.descriptor;

        // 필드 & 프로퍼티 세트 생성 (비어 있음)
        this.subFieldSet = new EventFieldSet(metadata.subFieldDescriptorArray);
        this.subPropertySet = new EventPropertySet(metadata.subPropertyDescriptorArray);

        // 서브 그룹 세트 생성
        this.subGroupSet = new EventDataGroupSet(metadata.subDataGroupDescriptorArray);
    }
}