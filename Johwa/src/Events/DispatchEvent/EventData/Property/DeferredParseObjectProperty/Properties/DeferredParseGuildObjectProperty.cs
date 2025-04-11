namespace Johwa.Event.Data;

public class DeferredParseGuildObjectProperty : DeferredParseObjectProperty
{
    public readonly DeferredParseObjectMetaData metaData;

    public DeferredParseGuildObjectProperty(DeferredParseObjectMetaData metaData, 
        ReadOnlyMemory<byte> data) : base(data) 
    { 
        this.metaData = metaData;
    }
}