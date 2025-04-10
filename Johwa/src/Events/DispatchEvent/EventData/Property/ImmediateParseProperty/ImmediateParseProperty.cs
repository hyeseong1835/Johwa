using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventDataMetadata
{
    public override EventDataAttribute Attribute => attribute;
    public ImmediateParsePropertyAttribute attribute;
    public FieldInfo fieldInfo;

    public ImmediateParsePropertyMetadata(ImmediateParsePropertyAttribute attribute, FieldInfo fieldInfo)
    {
        this.attribute = attribute;
        this.fieldInfo = fieldInfo;
    }

    public override void InitProperty(object obj, IEventData container, JsonTokenType tokenType)
    {
        fieldInfo.SetValue(obj, attribute.Parse(container));
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
    public abstract object? Parse(IEventData container);
}