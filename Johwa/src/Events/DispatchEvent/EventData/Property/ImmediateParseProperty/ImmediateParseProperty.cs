using System.Reflection;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventDataMetadata
{
    public override EventDataAttribute Attribute => attribute;
    public ImmediateParsePropertyAttribute attribute;
    public FieldInfo fieldInfo;

    public ImmediateParsePropertyMetadata(ImmediateParsePropertyAttribute attribute)
    {
        this.attribute = attribute;
        fieldInfo = attribute.fieldInfo;
    }

    public override void InitProperty(object obj, IEventData container)
    {
        fieldInfo.SetValue(obj, attribute.Parse(container));
    }
}
public abstract class ImmediateParsePropertyAttribute : EventDataAttribute
{
    public FieldInfo fieldInfo;

    public ImmediateParsePropertyAttribute(FieldInfo fieldInfo, 
        string name, bool isOptional = false) : base(name, isOptional) 
    {
        this.fieldInfo = fieldInfo; 
    }

    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new ImmediateParsePropertyMetadata(this);
    }
    public abstract object? Parse(IEventData container);
}