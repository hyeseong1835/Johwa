using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParseBoolAttribute : ImmediateParsePropertyAttribute
{
    public ImmediateParseBoolAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new ImmediateParsePropertyMetadata(this, fieldInfo);
    }
    public override object? Parse(ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        if (tokenType == JsonTokenType.True) {
            return true;
        } 
        else if (tokenType == JsonTokenType.False) {
            return false;
        } 
        else throw new JsonException($"Invalid token type for boolean: {tokenType}");
    }
}