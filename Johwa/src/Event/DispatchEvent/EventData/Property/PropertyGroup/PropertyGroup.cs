namespace Johwa.Event.Data;

public abstract class PropertyGroupData : EventPropertyData
{
    // 필드
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    // 생성자
    public PropertyGroupData(
        ReadOnlyMemory<byte> data, EventPropertyDescriptorAttribute[] propertyDescriptorArray) : base(data)
    {
        this.propertyDescriptorArray = propertyDescriptorArray;
    }

    // 메서드
    public abstract IEnumerable<EventPropertyData> GetPropertyDataEnumerable();
}