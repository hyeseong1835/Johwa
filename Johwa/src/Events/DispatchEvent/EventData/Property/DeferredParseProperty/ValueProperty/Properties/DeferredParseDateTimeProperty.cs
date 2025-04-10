using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseDateTimeProperty : DeferredParseValueProperty<DateTime>
{
    public DeferredParseDateTimeProperty(
        EventData data) : base(data) { }
    protected override DateTime Parse()
    {
        Utf8JsonReader reader = new Utf8JsonReader(data);

        return reader.GetDateTime();
    }
}