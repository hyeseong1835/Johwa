using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseValuePropertyMetaData : DeferredParseMetaData
{
    public override EventDataAttribute Attribute => attribute;
    public DeferredParseValueAttribute attribute;

    public DeferredParseValuePropertyMetaData(DeferredParseValueAttribute attribute)
    { 
        this.attribute = attribute;
    }
    public override void InitProperty(object obj, IEventData container, JsonTokenType tokenType) { }
}

public class DeferredParseValueAttribute : DeferredParseAttribute
{
    public DeferredParseValueAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }

    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new DeferredParseValuePropertyMetaData(this);
    }
}

public abstract class DeferredParseValueProperty<T> : DeferredParseProperty
{
    protected T? value = default;
    public bool isParsed = false;

    public DeferredParseValueProperty(EventData data) : base(data) 
    {

    }

    public T? Get()
    {
        if (isParsed == false) {
            value = Parse();
        }

        return value;
    }
    protected abstract T? Parse();
}