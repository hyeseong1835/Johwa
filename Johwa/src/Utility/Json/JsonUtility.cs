using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Johwa.Utility.Json;

public static class JsonUtility
{
    public static string ToPrettyJsonString(string dirtyJsonString)
    {
        dirtyJsonString = dirtyJsonString.TrimEnd('\0', ' ', '\r', '\n');

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(dirtyJsonString.Length);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(maxByteCount);

        try
        {
            int byteCount = Encoding.UTF8.GetBytes(dirtyJsonString, 0, dirtyJsonString.Length, buffer, 0);
            
            // 유효한 부분만 Slice 해서 넘김
            ReadOnlySpan<byte> jsonSpan = new ReadOnlySpan<byte>(buffer, 0, byteCount);
            string prettyJsonString = ToPrettyJsonString(jsonSpan);

            return prettyJsonString;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static string ToPrettyJsonString(byte[] utf8Json)
    {
        var reader = new Utf8JsonReader(utf8Json);

        using var outputStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(
            outputStream, 
            new JsonWriterOptions{ Indented = true }
        );

        WriteRecursive(ref reader, writer);

        writer.Flush();
        return Encoding.UTF8.GetString(outputStream.GetBuffer(), 0, (int)outputStream.Length);
    }
    public static string ToPrettyJsonString(ReadOnlySpan<byte> utf8Json)
    {
        var reader = new Utf8JsonReader(utf8Json);

        using var outputStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(
            outputStream,
            new JsonWriterOptions { Indented = true }
        );

        WriteRecursive(ref reader, writer);

        writer.Flush();
        return Encoding.UTF8.GetString(outputStream.GetBuffer(), 0, (int)outputStream.Length);
    }

    private static void WriteRecursive(ref Utf8JsonReader reader, Utf8JsonWriter writer)
    {
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    writer.WriteStartObject();
                    break;
                case JsonTokenType.EndObject:
                    writer.WriteEndObject();
                    break;
                case JsonTokenType.StartArray:
                    writer.WriteStartArray();
                    break;
                case JsonTokenType.EndArray:
                    writer.WriteEndArray();
                    break;
                case JsonTokenType.PropertyName:
                    writer.WritePropertyName(reader.GetString()?? string.Empty);
                    break;
                case JsonTokenType.String:
                    writer.WriteStringValue(reader.GetString());
                    break;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        writer.WriteNumberValue(l);
                    else
                        writer.WriteNumberValue(reader.GetDouble());
                    break;
                case JsonTokenType.True:
                    writer.WriteBooleanValue(true);
                    break;
                case JsonTokenType.False:
                    writer.WriteBooleanValue(false);
                    break;
                case JsonTokenType.Null:
                    writer.WriteNullValue();
                    break;
                default:
                    throw new JsonException($"Unhandled token: {reader.TokenType}");
            }
        }
    }
}