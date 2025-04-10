using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParseBoolAttribute : ImmediateParsePropertyAttribute
{
    public ImmediateParseBoolAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo, string name, bool isOptional)
    {
        return new ImmediateParsePropertyMetadata(this, fieldInfo, name, isOptional);
    }
    public override object? Parse(IEventData container)
    {
        Utf8JsonReader reader = new Utf8JsonReader(container.GetData().Span);
        return reader.GetBoolean();
    }
}