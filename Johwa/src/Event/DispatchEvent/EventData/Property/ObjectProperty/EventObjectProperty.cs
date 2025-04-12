using System.Reflection;

namespace Johwa.Event.Data;

public abstract class EventObjectPropertyMetadata : EventPropertyMetadata
{
    // 필드
    public readonly int minPropertyCount;

    // 필드
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    public EventObjectPropertyMetadata(EventPropertyDescriptorAttribute[] propertyDescriptorArray)
    {
        this.propertyDescriptorArray = propertyDescriptorArray;

        // 프로퍼티 최소 개수
        this.minPropertyCount = 0;
        for (int i = 0; i < propertyDescriptorArray.Length; i++)
        {
            EventPropertyDescriptorAttribute descriptor = propertyDescriptorArray[i];
            if (descriptor.isOptional == false)
                minPropertyCount++;
        }
    }
}
public abstract class EventObjectPropertyDescriptorAttribute : EventPropertyDescriptorAttribute
{
    // 필드
    public abstract EventObjectPropertyMetadata EventValuePropertyMetadata { get; }

    // 생성자
    public EventObjectPropertyDescriptorAttribute(
        FieldInfo fieldInfo, string name, bool isOptional, bool isNullable) : base(fieldInfo, name, isOptional, isNullable) { }
}
public abstract class EventObjectPropertyData : EventPropertyData
{
    // 필드
    public abstract EventValuePropertyDescriptorAttribute EventValuePropertyDescriptor { get; }

    // 생성자
    public EventObjectPropertyData(
        ReadOnlyMemory<byte> data) : base(data) { }
}