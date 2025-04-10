namespace Johwa.Event.Data;

public class ImmediateParseBoolPropertyAttribute : ImmediateParsePropertyAttribute
{
    public DeferredParsePropertyAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventPropertyMetadata CreateMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional)
    {
        return new DeferredParsePropertyMetaData(eventDataMetadata, fieldInfo, name, isOptional);
    }
    public void Set(EventDataDocument eventData, bool value)
    {
        eventData.SetBool(name, value);
    }
}