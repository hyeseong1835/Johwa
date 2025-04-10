using System.Text.Json;

namespace Johwa.Event.Data;

public class StringProperty : DeferredParseValueProperty<string>
{
    public StringProperty(EventDataMetadata metadata, EventDataDocument eventData, int startIndex, int length)
        : base(metadata, eventData, startIndex, length) { }

    protected override string Parse()
    {
        Utf8JsonReader reader = new Utf8JsonReader(Data);
        string? result = reader.GetString();
        if (result == null) {
            throw new JsonException($"Failed to parse string property '{metadata.name}'.");
        }
        return result;
    }
}