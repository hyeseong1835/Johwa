namespace Johwa.Event.Data;

public class EventDataGroup : IEventDataGroup
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
    public readonly EventFieldSet subFieldSet;
    public readonly EventPropertySet subPropertySet;
    public readonly EventDataGroupSet subGroupSet;

    // 생성자
    public EventDataGroup(CreateData createData)
    {
        // 필드 & 프로퍼티 세트 생성 (비어 있음)
        this.subFieldSet = new EventFieldSet(createData.descriptor.metadata.subFieldDescriptorArray);
        this.subPropertySet = new EventPropertySet(createData.descriptor.metadata.subPropertyDescriptorArray);

        // 서브 그룹 세트 생성
        this.subGroupSet = new EventDataGroupSet(createData.descriptor.metadata.subDataGroupDescriptorArray);
    }
}