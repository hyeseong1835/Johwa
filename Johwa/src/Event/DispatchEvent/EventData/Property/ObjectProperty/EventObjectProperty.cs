using System.Reflection;

namespace Johwa.Event.Data;

public abstract class EventObjectPropertyMetadata : EventPropertyMetadata, IEventDataContainerMetadata
{
    #region 재정의

    Type IEventDataContainerMetadata.ContainerType => dataType;

    public EventPropertyGroupDescriptorAttribute[] PropertyGroupDescriptorArray => propertyGroupDescriptorArray;

    public EventPropertyDescriptorAttribute[] PropertyDescriptorArray => propertyDescriptorArray;

    #endregion


    // 필드
    public Type dataType;
    public readonly int minPropertyCount;
    public readonly EventPropertyGroupDescriptorAttribute[] propertyGroupDescriptorArray;
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    // 생성자
    public EventObjectPropertyMetadata(Type dataType)
    {
        this.dataType = dataType;
        this.propertyDescriptorArray = IEventDataContainer.LoadPropertyDataDescriptors(dataType).ToArray();
        this.propertyGroupDescriptorArray = IEventDataContainer.LoadPropertyGroupDescriptors(dataType).ToArray();

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