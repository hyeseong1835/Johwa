using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseDateTimeProperty : DeferredParseValueProperty<DateTime>
{
    public DeferredParseDateTimeProperty(
        ReadOnlyMemory<byte> data) : base(data) { }
        
    protected override DateTime Parse()
    {
        Utf8JsonReader reader = new Utf8JsonReader(data.Span);

        return reader.GetDateTime();
    }
}