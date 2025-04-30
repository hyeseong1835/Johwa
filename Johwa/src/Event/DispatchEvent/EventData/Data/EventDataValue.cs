using System.Text.Json;

namespace Johwa.Event.Data;

public struct EventDataValue
{
    public ReadOnlyMemory<byte> jsonValue;
    public JsonTokenType jsonTokenType;

    public EventDataValue(ReadOnlyMemory<byte> jsonValue, JsonTokenType jsonTokenType)
    {
        this.jsonValue = jsonValue;
        this.jsonTokenType = jsonTokenType;
    }
}