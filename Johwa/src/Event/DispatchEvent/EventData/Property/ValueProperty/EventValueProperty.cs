using System.Reflection;

namespace Johwa.Event.Data;

public abstract class EventValuePropertyMetadata : EventPropertyMetadata
{

}
public abstract class EventValuePropertyDescriptorAttribute : EventPropertyDescriptorAttribute
{
    // 필드
    public abstract EventValuePropertyMetadata EventValuePropertyMetadata { get; }

    // 생성자
    public EventValuePropertyDescriptorAttribute(
        FieldInfo fieldInfo, string name, bool isOptional, bool isNullable) : base(fieldInfo, name, isOptional, isNullable) { }
}
public abstract class EventValuePropertyData : EventPropertyData
{
    // 필드
    public abstract EventValuePropertyDescriptorAttribute EventValuePropertyDescriptor { get; }

    // 생성자
    public EventValuePropertyData(ReadOnlyMemory<byte> data) : base(data) { }
}