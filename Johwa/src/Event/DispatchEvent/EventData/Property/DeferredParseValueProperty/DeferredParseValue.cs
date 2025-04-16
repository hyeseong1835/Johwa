namespace Johwa.Event.Data.Property.DeferredParseValueProperty;

public class DeferredParseValue : EventProperty
{
    public override EventPropertyDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new DeferredParseValueMetadata(fieldInfo.FieldType);
    }
}