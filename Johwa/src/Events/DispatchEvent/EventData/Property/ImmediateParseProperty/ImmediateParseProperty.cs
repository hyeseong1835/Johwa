using System.Reflection;

namespace Johwa.Event.Data;

public class ImmediateParsePropertyMetadata : EventPropertyMetadata
{
    public ImmediateParsePropertyMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional) 
        : base(eventDataMetadata, fieldInfo, name, isOptional) { }
}
public abstract class ImmediateParsePropertyAttribute : EventDataPropertyAttribute
{
    public ImmediateParsePropertyAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventPropertyMetadata CreateMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional)
    {
        return new DeferredParseValuePropertyMetaData(eventDataMetadata, fieldInfo, name, isOptional);
    }
}