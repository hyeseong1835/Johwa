using System.Text.Json;
using System.Text.Json.Serialization;

namespace Johwa.Common;

/// <summary>
/// JSON 직렬화/역직렬화용 컨버터
/// </summary>
public class SnowflakeJsonConverter : JsonConverter<Snowflake>
{
    public override Snowflake Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && ulong.TryParse(reader.GetString(), out var value))
        {
            return new Snowflake(value);
        }
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetUInt64(out var ulongValue))
        {
            return new Snowflake(ulongValue);
        }

        throw new JsonException("Invalid snowflake value.");
    }

    public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}