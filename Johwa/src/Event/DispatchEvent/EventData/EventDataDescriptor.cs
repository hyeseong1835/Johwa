using System.Text.Json;

namespace Johwa.Event.Data;

public abstract class EventDataInfo
{
    public EventDataMetadata metadata;
    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventDataInfo(EventDataMetadata metadata, string name, bool isOptional, bool isNullable)
    {
        this.metadata = metadata;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }
    
    unsafe public abstract void ReadData<TDocument>(TDocument* documentPtr, ReadOnlyMemory<byte> jsonData, JsonTokenType jsonTokenType)
        where TDocument : unmanaged, IEventDataDocument;
}