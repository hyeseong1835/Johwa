namespace Johwa.Event.Data;

public abstract class EventDataDescriptor
{
    public EventDataMetadata metadata;
    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventDataDescriptor(EventDataMetadata metadata, string name, bool isOptional, bool isNullable)
    {
        this.metadata = metadata;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }
    
    public abstract EventData? CreateData(EventData.EventDataCreateData createData);
}