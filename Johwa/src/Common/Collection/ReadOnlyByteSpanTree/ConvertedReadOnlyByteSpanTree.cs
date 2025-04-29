using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Collection;

public partial class ReadOnlyByteSpanTree<TValue>
{
    unsafe public struct ConvertedByteSpanTree<TConvertedValue> : IDisposable
        where TConvertedValue: unmanaged
    {
        ReadOnlyByteSpanTree<TValue> originalTree;
        GCHandle originalTreeGCHandle;

        TConvertedValue* valueArray;
        Func<TValue, TConvertedValue> valueConverter;

        public ConvertedByteSpanTree(ReadOnlyByteSpanTree<TValue> originalTree, Func<TValue, TConvertedValue> valueConverter)
        {
            this.originalTree = originalTree;
            originalTreeGCHandle = GCHandle.Alloc(originalTree, GCHandleType.Pinned);

            this.valueConverter = valueConverter;

            valueArray = (TConvertedValue*)Marshal.AllocHGlobal(sizeof(TConvertedValue) * originalTree.valueCount);
            ReadOnlyByteSpanTree<TValue>.ValueIterator valueIterator = originalTree.GetValueIterator();
            for (int i = 0; i < originalTree.valueCount; i++)
            {
                TValue? originalValue;
                if (valueIterator.MoveNext(out originalValue))
                {
                    valueArray[i] = valueConverter.Invoke(originalValue);
                }
                else
                {
                    throw new InvalidOperationException("원본 트리의 값 수가 일치하지 않습니다.");
                }
            }
        }
        public TConvertedValue GetValue(ReadOnlySpan<byte> key)
        {
            Node originalNode = originalTree.GetNode(key);
            return valueArray[originalNode.valueIndex];
        }

        public void Dispose()
        {
            originalTreeGCHandle.Free();

            Marshal.FreeHGlobal((IntPtr)valueArray);
            valueArray = null;
        }
    }
}