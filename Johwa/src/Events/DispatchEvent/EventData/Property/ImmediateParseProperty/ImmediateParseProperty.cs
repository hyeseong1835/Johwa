using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventPropertyMetadata
{
    public override EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        => new ImmediateParsePropertyData(this, container, data, tokenType);
}
public abstract class ImmediateParsePropertyDescriptorAttribute : EventPropertyDescriptorAttribute
{
    // 재정의
    public override Type PropertyMetaDataType => typeof(ImmediateParsePropertyMetadata);
    public override EventPropertyMetadata PropertyMetaData => propertyMetadata;
    
    public override EventPropertyMetadata CreateMetadata(FieldInfo fieldInfo)
        => new ImmediateParsePropertyMetadata(this, fieldInfo);

    // 필드 & 프로퍼티
    public ImmediateParsePropertyMetadata propertyMetadata;

    // 생성자
    public ImmediateParsePropertyDescriptorAttribute(
        string name, bool isOptional = false, bool isNullable = false) : base(name, isOptional, isNullable) 
    {
        EventPropertyMetadata propertyMetadata = EventPropertyMetadata.GetInstance<ImmediateParsePropertyMetadata>();
        this.propertyMetadata = EventPropertyMetadata.GetMetadata(this, typeof(ImmediateParsePropertyMetadata), fieldInfo);
    }

    // 메서드
    public abstract object? Parse(ReadOnlyMemory<byte> data, JsonTokenType tokenType);
}
public class ImmediateParsePropertyData : EventPropertyData
{
    // 재정의
    public override EventPropertyDescriptorAttribute Descriptor => descriptor;

    // 필드
    public ImmediateParsePropertyDescriptorAttribute descriptor;

    public ImmediateParsePropertyData(ImmediateParsePropertyDescriptorAttribute descriptor, 
        IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType) : base(descriptor, data) 
    {
        this.descriptor = descriptor;
        EventPropertyMetadata metadata = descriptor.PropertyMetaData;

        metadata.fieldInfo.SetValue(container, metadata.attribute.Parse(data, tokenType));
    }
}