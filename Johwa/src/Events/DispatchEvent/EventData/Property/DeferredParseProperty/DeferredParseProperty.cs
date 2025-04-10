namespace Johwa.Event.Data;

public abstract class DeferredParseMetaData : EventDataMetadata
{
    
}

public abstract class DeferredParseAttribute : EventDataAttribute
{
    public DeferredParseAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }
}

public abstract class DeferredParseProperty : EventProperty
{
    public DeferredParseProperty(
        EventData data) : base(data) { }
}