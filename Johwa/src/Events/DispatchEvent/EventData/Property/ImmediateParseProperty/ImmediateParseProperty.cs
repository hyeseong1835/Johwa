using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventDataMetadata
{
    public override EventDataAttribute Attribute => attribute;
    public ImmediateParsePropertyAttribute attribute;

    public ImmediateParsePropertyMetadata(ImmediateParsePropertyAttribute attribute, 
        FieldInfo fieldInfo) : base(fieldInfo)
    {
        this.attribute = attribute;
    }

    public override void InitProperty(object obj, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        fieldInfo.SetValue(obj, attribute.Parse(data, tokenType));
    }
}
public abstract class ImmediateParsePropertyAttribute : EventDataAttribute
{
    public ImmediateParsePropertyAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new ImmediateParsePropertyMetadata(this, fieldInfo);
    }
    public abstract object? Parse(ReadOnlyMemory<byte> data, JsonTokenType tokenType);
}