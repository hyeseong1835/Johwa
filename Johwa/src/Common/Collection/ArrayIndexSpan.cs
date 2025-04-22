namespace Johwa.Common.Collection;

/// <summary>
/// ArrayPool 기반으로 배열의 인덱스를 저장합니다.
/// </summary>
public ref struct ArrayIndexSpan<T>
{
    public Span<T> originalValueSpan;
    public Span<int> indexSpan;

    public int Count => count;
    public int count;

    public bool IsReadOnly => false;

    // 생성자
    public ArrayIndexSpan(Span<T> originalValueSpan, Span<int> indexBuffer)
    {
        this.originalValueSpan = originalValueSpan;
        this.indexSpan = indexBuffer;
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

        return originalValueSpan[valueIndex];
    }
    public T SetValue(int index, T value)
    {
        int valueIndex = GetIndex(index);

        originalValueSpan[valueIndex] = value;
        return value;
    }
    public void Add(int valueIndex)
    {
        if (count >= indexSpan.Length) throw new InvalidOperationException("ArrayIndexSet is full.");

        indexSpan[count++] = valueIndex;
    }
}