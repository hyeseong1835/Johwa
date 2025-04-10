namespace Johwa.Event.Data;

public class DeferredParseGuildObjectProperty : DeferredParseObjectProperty
{
    public static DeferredParseObjectMetaData? metaData;

    public DeferredParseGuildObjectProperty(
        EventData eventData) : base(eventData) { }
    
    public override void Init()
    {
        if (metaData == null) {
            metaData = new DeferredParseObjectMetaData();
        }
    }
}