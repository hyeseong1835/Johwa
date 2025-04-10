using System.Text.Json;

namespace Johwa.Event.Data;

public class StringProperty : DeferredParseValueProperty<string>
{
    public StringProperty(EventData data)
        : base(data) { }

    protected override string Parse()
    {
        Utf8JsonReader reader = new Utf8JsonReader(data);
        string? result = reader.GetString();
        if (result == null) {
            throw new JsonException($"Failed to parse string property '{data}'.");
        }
        return result;
    }
}