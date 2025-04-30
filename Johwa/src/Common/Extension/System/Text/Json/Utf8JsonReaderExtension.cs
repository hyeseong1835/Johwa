using System.ComponentModel;
using System.Text.Json;

namespace Johwa.Common.Extension.System.Text.Json;
public static class Utf8JsonReaderExtension
{
    public static bool TryFindPropertyName(this ref Utf8JsonReader reader, string propertyName)
    {
        int startDepth = reader.CurrentDepth;

        while (reader.Read())
        {
            // 현재 영역을 벗어나면 종료
            if (reader.CurrentDepth < startDepth)
                return false;

            // 객체 Skip
            if (reader.TokenType == JsonTokenType.StartObject) {
                reader.Skip(); 
                continue;
            }
            
            // 프로퍼티 이름만 필터링
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue; 

            if (reader.ValueTextEquals(propertyName))
                return true; // 찾음
        }
        
        // 끝까지 읽었지만 찾지 못함
        return false; 
    }
    public static bool ReadValueByte(this ref Utf8JsonReader reader, ReadOnlySpan<byte> originalJson, out ReadOnlySpan<byte> result)
    {
        // 현재 토큰의 시작 인덱스를 저장합니다.
        int startIndex = (int)reader.TokenStartIndex;

        switch (reader.TokenType)
        {
            // 단일 값 타입
            case JsonTokenType.PropertyName: 
            case JsonTokenType.String: 
            case JsonTokenType.Number: 
            case JsonTokenType.True: 
            case JsonTokenType.False: 
            case JsonTokenType.Null: 
            case JsonTokenType.Comment: 
            {
                // 다음 값으로 이동
                if(reader.Read()) 
                {
                    break;
                }
                else
                {
                    result = default;
                    return false;
                }
            }

            // 다중 값 타입
            case JsonTokenType.StartObject:
            case JsonTokenType.StartArray:
            {
                reader.Skip(); // 객체 Skip

                // 다음 값으로 이동
                if(reader.Read()) 
                {
                    break;
                }
                else
                {
                    result = default;
                    return false;
                }
            }

            default:
                throw new InvalidEnumArgumentException($"지원하지 않는 토큰 타입입니다: {reader.TokenType}");
        }

        // 현재까지 소비된 바이트 수를 저장합니다. 
        int endIndex = (int)reader.BytesConsumed;

        // 원본 JSON 바이트 메모리에서 해당 범위를 슬라이스하여 반환합니다. 
        result = originalJson.Slice(startIndex, endIndex - startIndex);
        return true;
    }
}