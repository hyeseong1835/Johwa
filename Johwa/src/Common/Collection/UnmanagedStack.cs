using System.Runtime.InteropServices;

namespace Johwa.Common.Collection;

/// <summary>
/// UnmanagedStack는 관리되지 않는 메모리에서 스택을 구현합니다. <br/>
/// <br/>
/// 이 구조체를 더 이상 사용하지 않을 때 Dispose()를 호출해야합니다.
/// </summary>
/// <typeparam name="T"></typeparam>
unsafe public struct UnmanagedStack<T> : IDisposable
    where T : unmanaged
{
    T* stack;
    int topIndex;
    int size;
    public int Count => topIndex + 1;
    public bool IsEmpty => topIndex < 0;

    public UnmanagedStack(int size)
    {
        this.size = size;
        this.topIndex = -1;

        // 관리되지 않는 힙에 메모리 할당
        this.stack = (T*)Marshal.AllocHGlobal(sizeof(T) * size);
    }

    public ref T Push(T value)
    {
        if (topIndex + 1 >= size)
            throw new InvalidOperationException("버퍼가 가득 찼습니다.");

        // 다음 위치로 이동
        topIndex++;
        ref T valueRef = ref stack[topIndex];

        // 값 할당
        valueRef = value;

        // 참조 반환
        return ref valueRef;
    }
    
    public ref T Pop()
    {
        if (topIndex < 0)
            throw new InvalidOperationException("Stack이 비었습니다.");

        return ref stack[topIndex--];
    }

    public ref T Peek()
    {
        if (topIndex < 0)
            throw new InvalidOperationException("Stack이 비었습니다.");

        return ref stack[topIndex];
    }

    public void Dispose()
    {
        IntPtr stackIntPtr = (IntPtr)stack;

        if (stackIntPtr == IntPtr.Zero)
        {
            throw new ObjectDisposedException("ValueStack<T>");
        }
        
        Marshal.FreeHGlobal(stackIntPtr);
        stack = null;
    }
}