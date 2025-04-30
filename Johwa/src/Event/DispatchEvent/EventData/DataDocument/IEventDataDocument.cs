using System.Runtime.InteropServices;
using System.Text.Json;

namespace Johwa.Event.Data;

/// <summary>
/// 이벤트 데이터  <br/>
/// <br/>
/// 이것을 상속받는 객체는 관리되지 않는 타입이여야합니다.  <br/>
/// 반드시 속성 [StructLayout(LayoutKind.Sequential)]을 가져야합니다.
/// </summary>
unsafe public interface IEventDataDocument
{
    public static ref TDocument CreateDocument<TDocument>(ReadOnlyMemory<byte> jsonData)
        where TDocument: unmanaged, IEventDataDocument
    {
        Type documentType = typeof(TDocument);
        EventDataDocumentMetadata metadata = EventDataDocumentMetadata.GetOrCreateInstance(documentType);

        TDocument* documentPtr = (TDocument*)Marshal.AllocHGlobal(sizeof(TDocument));
        
        // Json 읽기
        ReadOnlySpan<byte> jsonDataSpan = jsonData.Span;
        Utf8JsonReader jsonReader = new(jsonDataSpan);

        while (jsonReader.Read())
        {
            // 이름만 필터링
            if (jsonReader.TokenType != JsonTokenType.PropertyName) continue;

            // 데이터 이름
            ReadOnlySpan<byte> jsonDataNameSpan = jsonReader.ValueSpan;

            // 값으로 이동
            if (jsonReader.Read()) 
                throw new JsonException($"프로퍼티의 값이 존재하지 않습니다. {jsonDataNameSpan.ToString()}");

            JsonTokenType valueTokenType = jsonReader.TokenType;

            switch (valueTokenType)
            {
                // 객체 Skip
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    jsonReader.Skip();
                    break;
                
                // 나머지 타입은 무시
                default:
                    break;
            }
            

            if (metadata.dataInfoTree.TryGetValue(jsonDataNameSpan, out EventDataInfo? dataInfo))
            {
                // jsonData에서 값 부분만 슬라이스
                JsonTokenType jsonTokenType = jsonReader.TokenType;
                ReadOnlyMemory<byte> dataJsonData = jsonReader.ReadAndSliceValue(jsonData);

                EventDataValue dataValue = new (
                    dataJsonData, 
                    jsonReader.TokenType
                );

                // 값 읽기
                dataInfo.ReadData(
                    documentPtr, 
                    dataValue
                );
            }
            else
            {
                switch (jsonReader.TokenType)
                {
                    // 객체 Skip
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        jsonReader.Skip();
                        break;
                    
                    // 나머지 타입은 무시
                    default:
                        break;
                }
            }
        }
        return ref *documentPtr;
    }
}