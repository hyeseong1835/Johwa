using System.Runtime.InteropServices;

namespace Johwa.Common.Collection;

/// <summary>
/// 관리되지 않는 메모리에서 싱글 링크드 리스트 기반 스택을 구현합니다. <br/>
/// <br/>
/// 더 이상 사용하지 않을 때 Dispose()를 호출해야합니다.
/// </summary>
/// <typeparam name="T"></typeparam>
unsafe public struct UnmanagedStack<T> : IDisposable
    where T : unmanaged
{
    UnmanagedLinkedList<T> list;
    public int Count => list.Count;
    public bool IsEmpty => list.IsEmpty;

    public UnmanagedStack()
    {
        list = new UnmanagedLinkedList<T>();
    }

    public ref T Push(T value)
        => ref list.AddFirst(value);
    
    public void Pop()
        => list.RemoveFirst();

    public ref T Peek()
        => ref list.HeadValueRef;

    public void Dispose()
        => list.Dispose();
}