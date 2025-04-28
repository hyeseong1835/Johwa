using System.Runtime.InteropServices;

namespace Johwa.Event.Data;

/// <summary>
/// 이벤트 데이터  <br/>
/// <br/>
/// 이것을 상속받는 객체는 관리되지 않는 타입이여야합니다.  <br/>
/// 반드시 속성 [StructLayout(LayoutKind.Sequential)]을 가져야합니다.
/// </summary>
unsafe public interface IEventDataDocument
{
    public static TDocument* CreateDocument<TDocument>(ReadOnlyMemory<byte> jsonData)
        where TDocument: unmanaged, IEventDataDocument
    {
        Type documentType = typeof(TDocument);
        EventDataDocumentMetadata metadata = EventDataDocumentMetadata.GetOrCreateInstance(documentType);

        TDocument* documentPtr = (TDocument*)Marshal.AllocHGlobal(sizeof(TDocument));
        
    }
}