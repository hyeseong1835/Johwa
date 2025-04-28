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
    
    /// <summary>
    /// # StartObject, StartArray <br/>
    /// - EndObject, EndArray로 이동함. <br/>
    /// <br/>
    /// # 나머지 타입 <br/>
    /// - 움직이지 않음
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="originalJson"></param>
    /// <returns></returns>
    public static ReadOnlyMemory<byte> ReadAndSliceValue(this ref Utf8JsonReader reader, ReadOnlyMemory<byte> originalJson)
    {
        // 값 시작 위치 기록
        int start = (int)reader.TokenStartIndex;

        // 다음 값으로 이동
        switch (reader.TokenType)
        {
            // 다중 값 타입
            case JsonTokenType.StartObject:
            case JsonTokenType.StartArray: 
            {
                // 객체 Skip 
                reader.Skip(); 

                // 값 끝 위치 기록
                int end = (int)reader.BytesConsumed;

                // 원본 자르기
                return originalJson.Slice(start, end - start);
            }

            // 단일 값 타입
            default: 
            {
                // 원본 자르기
                return originalJson.Slice(start, reader.ValueSpan.Length);
            }
        }

        
    }
}