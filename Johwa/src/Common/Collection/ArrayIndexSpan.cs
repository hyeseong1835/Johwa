using System.Buffers;

namespace Johwa.Common.Collection;

/// <summary>
/// ArrayPool 기반으로 배열의 인덱스를 저장합니다.
/// </summary>
public ref struct ArrayIndexSpan<T>
{
    T[] originalValueArray;
    Span<int> indexSpan;

    int count;
    public int Count => count;

    public ArrayIndexSpan(T[] originalArray, Span<int> indexSpan)
    {
        this.originalValueArray = originalArray;
        this.indexSpan = indexSpan;
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

        return indexSpan[index];
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
        if (count >= indexSpan.Length) throw new InvalidOperationException("ArrayIndexSet is full.");

        indexSpan[count++] = valueIndex;
    }
}