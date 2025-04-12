using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventPropertyTypeMetadata
{
    public override EventPropertyAttribute Attribute => attribute;
    public override EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        => new ImmediateParsePropertyData(this, container, data, tokenType);

    public ImmediateParsePropertyAttribute attribute;

    public ImmediateParsePropertyMetadata(ImmediateParsePropertyAttribute attribute, 
        FieldInfo fieldInfo) : base(fieldInfo)
    {
        this.attribute = attribute;
    }


}
public abstract class ImmediateParsePropertyAttribute : EventPropertyAttribute
{
    // 재정의
    public override Type PropertyMetaDataType => typeof(ImmediateParsePropertyMetadata);
    public override EventPropertyTypeMetadata CreateMetadata(FieldInfo fieldInfo)
        => new ImmediateParsePropertyMetadata(this, fieldInfo);

    // 생성자
    public ImmediateParsePropertyAttribute(
        string name, bool isOptional = false, bool isNullable = false) : base(name, isOptional, isNullable) { }

    // 추상 메서드
    public abstract object? Parse(ReadOnlyMemory<byte> data, JsonTokenType tokenType);
}
public class ImmediateParsePropertyData : EventPropertyData
{
    public ImmediateParsePropertyData(
        ImmediateParsePropertyMetadata metadata, IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType) : base(metadata, data) 
    { 
        metadata.fieldInfo.SetValue(container, metadata.attribute.Parse(data, tokenType));
    }
}