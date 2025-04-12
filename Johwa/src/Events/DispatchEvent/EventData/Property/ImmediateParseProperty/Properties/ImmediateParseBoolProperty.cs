using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParseBoolAttribute : ImmediateParsePropertyDescriptorAttribute
{
    public ImmediateParseBoolAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override object? Parse(ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        if (tokenType == JsonTokenType.True) {
            return true;
        } 
        if (tokenType == JsonTokenType.False) {
            return false;
        } 
        
        throw new JsonException($"Invalid token type for boolean: {tokenType}");
    }
}