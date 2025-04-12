using System.Text.Json;

namespace Johwa.Common.Extension.System.Text.Json;
public static class Utf8JsonReaderExtension
{
    public static bool TryFindPropertyName(this ref Utf8JsonReader reader, string propertyName)
    {
        int startDepth = reader.CurrentDepth;

        while (reader.Read())
        {
            if (reader.CurrentDepth < startDepth)
                return false; // 영역을 벗어남
            
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue; // 이름만 필터링

            if (reader.ValueTextEquals(propertyName))
                return true; // 찾음
        }
        return false; // 끝까지 읽었지만 찾지 못함
    }
    public static bool TryReadObject(this ref Utf8JsonReader reader, ReadOnlySpan<byte> originalJson, out ReadOnlySpan<byte> result)
    {
        if (reader.TokenType != JsonTokenType.StartObject){
            result = default;
            return false;
        }

        // 객체 시작 위치 기록
        int start = (int)reader.TokenStartIndex; 

        int depth = reader.CurrentDepth;

        // 객체가 끝날 때까지 반복하여 읽기
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == depth - 1)
            {
                // 객체의 끝 위치
                int end = (int)reader.BytesConsumed; 

                // 원본 JSON에서 객체에 해당하는 부분을 추출
                result = originalJson.Slice(start, end - start);
                return true;
            }
        }
        result = default;
        return false;
    }
    
    public static ReadOnlyMemory<byte> ReadAndSliceToken(this ref Utf8JsonReader reader, ReadOnlyMemory<byte> originalJson)
    {
        int start = (int)reader.TokenStartIndex;

        reader.Skip(); // 객체 Skip 

        int end = (int)reader.BytesConsumed;

        return originalJson.Slice(start, end - start);
    }
}