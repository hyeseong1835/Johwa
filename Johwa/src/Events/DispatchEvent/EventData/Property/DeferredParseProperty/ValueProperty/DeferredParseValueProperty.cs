using System.Reflection;

namespace Johwa.Event.Data;

public class DeferredParseValuePropertyMetaData : EventPropertyMetadata
{
    public DeferredParseValuePropertyMetaData(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional) 
        : base(eventDataMetadata, fieldInfo, name, isOptional) { }
}
public class DeferredParseValuePropertyAttribute : EventDataPropertyAttribute
{
    public DeferredParseValuePropertyAttribute(string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventPropertyMetadata CreateMetadata(EventDataDocumentMetadata eventDataMetadata, FieldInfo fieldInfo, string name, bool isOptional)
    {
        return new DeferredParseValuePropertyMetaData(eventDataMetadata, fieldInfo, name, isOptional);
    }
}

public abstract class DeferredParseValueProperty<T> : EventDataObjectUnit
{
    public readonly EventPropertyMetadata metadata;

    protected T? value = default;
    public bool isParsed = false;

    public DeferredParseValueProperty(EventPropertyMetadata metadata, EventDataDocument eventData, int startIndex, int length)
        : base(eventData, startIndex, length)
    {
        this.metadata = metadata;
    }

    public T? Get()
    {
        if (isParsed == false) {
            value = Parse(eventData);
        }

        return value;
    }
    protected abstract T? Parse(EventDataDocument eventData);

    public static implicit operator T(DeferredParseValueProperty<T> property)
    {
        return property.Get()!;
    }
}