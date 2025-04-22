using Johwa.Common.Collection;

namespace Johwa.Common.Extension.Johwa.Common.Collection;

public static class ArrayIndexSpanExtension
{
    public static ArrayIndexSpan<T> AsArrayIndexSpan<T>(this T[] array, Span<int> indexBuffer)
    {
        ArrayIndexSpan<T> result = new(array, indexBuffer);
        for (int i = 0; i < array.Length; i++)
        {
            result.Add(i);
        }

        return result;
    }
}