using System.Buffers;
using System.Collections;

namespace Johwa.Common.Collection;

/// <summary>
/// ArrayPool 기반으로 배열의 인덱스를 저장합니다.
/// </summary>
public class ArrayIndexSet<T> : IList<T>
{
    public IList<T> originalValueList;
    int[] indexArray;

    int count;
    public int Count => count;

    public bool IsReadOnly => false;

    // 생성자
    public ArrayIndexSet(IList<T> originalValueList, int maxCount)
    {
        this.originalValueList = originalValueList;
        count = 0;
        indexArray = ArrayPool<int>.Shared.Rent(maxCount);
    }
    public void Reset()
    {
        count = 0;
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

        return originalValueList[valueIndex];
    }
    public T SetValue(int index, T value)
    {
        int valueIndex = GetIndex(index);

        originalValueList[valueIndex] = value;
        return value;
    }
    public void Add(int valueIndex)
    {
        if (count >= indexArray.Length) throw new InvalidOperationException("ArrayIndexSet is full.");

        indexArray[count++] = valueIndex;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++)
        {
            yield return GetValue(i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }
}