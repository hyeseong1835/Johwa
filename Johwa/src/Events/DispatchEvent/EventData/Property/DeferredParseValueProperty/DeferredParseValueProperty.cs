using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseValuePropertyMetaData : EventPropertyTypeMetadata
{
    public override EventPropertyAttribute Attribute => attribute;
    public DeferredParseValueAttribute attribute;

    public DeferredParseValuePropertyMetaData(DeferredParseValueAttribute attribute,
        FieldInfo fieldInfo) : base(fieldInfo) 
    {
        this.attribute = attribute;
    }
    public override void InitProperty(object obj, ReadOnlyMemory<byte> data, JsonTokenType tokenType) { }
}

public class DeferredParseValueAttribute : EventPropertyAttribute
{
    public DeferredParseValueAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventPropertyTypeMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new DeferredParseValuePropertyMetaData(this, fieldInfo);
    }
}

public abstract class DeferredParseValueProperty<T> : EventPropertyData
{
    protected T? value = default;
    public bool isParsed = false;

    public DeferredParseValueProperty(
        ReadOnlyMemory<byte> data) : base(data) { }

    public T? Get()
    {
        if (isParsed == false) {
            value = Parse();
        }

        return value;
    }
    protected abstract T? Parse();
}