using System.Runtime.InteropServices;
using System.Text.Json;
using Johwa.Common.Extension.System.Text.Json;

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

            if (metadata.dataInfoTree.TryGetValue(jsonDataNameSpan, out EventDataInfo? dataInfo))
            {
                // jsonData에서 값 부분만 슬라이스
                ReadOnlyMemory<byte> dataJsonData = jsonReader.ReadAndSliceValue(jsonData);

                // 값 읽기
                dataInfo.ReadData(
                    documentPtr, 
                    dataJsonData, 
                    jsonReader.TokenType
                );
            }
        }
        return ref *documentPtr;
    }
}