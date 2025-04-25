#pragma warning disable CS8500 // 주소를 가져오거나, 크기를 가져오거나, 관리되는 형식에 대한 포인터를 선언합니다.

using System.Runtime.InteropServices;

namespace Johwa.Common.Collection;

unsafe public struct TempByteSpanTree<TValue, TOriginalValue> : IDisposable
{
    ReadOnlyByteSpanTree<TOriginalValue> originalTree;
    GCHandle originalTreeGCHandle;

    TValue* valueArray;
    Func<TOriginalValue, TValue> valueCreator;

    public TempByteSpanTree(ReadOnlyByteSpanTree<TOriginalValue> originalTree, Func<TOriginalValue, TValue> valueCreator)
    {
        this.originalTree = originalTree;
        originalTreeGCHandle = GCHandle.Alloc(originalTree, GCHandleType.Pinned);

        this.valueCreator = valueCreator;

        valueArray = (TValue*)Marshal.AllocHGlobal(sizeof(TValue) * originalTree.valueCount);
        ReadOnlyByteSpanTree<TOriginalValue>.ValueIterator valueIterator = originalTree.GetValueIterator();
        for (int i = 0; i < originalTree.valueCount; i++)
        {
            TOriginalValue? originalValue;
            if (valueIterator.MoveNext(out originalValue))
            {
                valueArray[i] = valueCreator.Invoke(originalValue);
            }
            else
            {
                throw new InvalidOperationException("원본 트리의 값 수가 일치하지 않습니다.");
            }
        }
    }

    public void Dispose()
    {
        originalTreeGCHandle.Free();

        Marshal.FreeHGlobal((IntPtr)valueArray);
        valueArray = null;
    }
}