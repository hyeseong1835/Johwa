using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseDateTimeProperty : DeferredParseValueProperty<DateTime>
{
    public DeferredParseDateTimeProperty(EventPropertyMetadata metadata, EventDataDocument eventData, int startIndex, int length)
        : base(metadata, eventData, startIndex, length) { }
    protected override DateTime Parse(EventDataDocument eventData)
    {
        Utf8JsonReader reader = new Utf8JsonReader(eventData.Data);

        return reader.GetDateTime();
    }
}