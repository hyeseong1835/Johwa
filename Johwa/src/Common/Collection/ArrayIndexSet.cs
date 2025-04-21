using System.Buffers;

namespace Johwa.Common.Collection;

/// <summary>
/// ArrayPool 기반으로 배열의 인덱스를 저장합니다.
/// </summary>
public class ArrayIndexSet<T>
{
    T[] originalValueArray;
    int[] indexArray;

    int count;
    public int Count => count;

    // 생성자
    public ArrayIndexSet(T[] originalArray, int maxCount)
    {
        this.originalValueArray = originalArray;
        count = 0;
        indexArray = ArrayPool<int>.Shared.Rent(maxCount);
    }
    public void Reset(int maxCount)
    {
        if (maxCount > indexArray.Length)
        {
            ArrayPool<int>.Shared.Return(indexArray);
            indexArray = ArrayPool<int>.Shared.Rent(maxCount);
        }
        
        count = 0;
    }
    public T this[int index]
    {
        get => GetValue(index);
        set => SetValue(index, value);
    }
    public int GetIndex(int index)
    {
        if (index < 0 || index >= count) throw new ArgumentOutOfRangeException(nameof(index));

        return indexArray[index];
    }
    public T GetValue(int index)
    {
        int valueIndex = GetIndex(index);

        return originalValueArray[valueIndex];
    }
    public T SetValue(int index, T value)
    {
        int valueIndex = GetIndex(index);

        originalValueArray[valueIndex] = value;
        return value;
    }
    public void Add(int valueIndex)
    {
        if (count >= indexArray.Length) throw new InvalidOperationException("ArrayIndexSet is full.");

        indexArray[count++] = valueIndex;
    }
}