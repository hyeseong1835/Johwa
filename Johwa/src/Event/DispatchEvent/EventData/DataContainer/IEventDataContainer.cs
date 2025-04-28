namespace Johwa.Event.Data;

/// <summary>
/// 이벤트 데이터의 오브젝트 값  <br/>
/// <br/>
/// 이것을 상속받는 객체는 관리되지 않는 타입이여야합니다.  <br/>
/// 반드시 속성 [StructLayout(LayoutKind.Sequential)]을 가져야합니다.
/// </summary>
public interface IEventDataContainer
{
    
}