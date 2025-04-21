using Johwa.Common.Collection;

namespace Johwa.Common.Extension.Johwa.Common.Collection;

public static class ArrayIndexSetExtension
{
    public static ArrayIndexSet<T> ToArrayIndexSet<T>(this T[] array)
    {
        ArrayIndexSet<T> result = new(array, array.Length);
        for (int i = 0; i < array.Length; i++)
        {
            result.Add(i);
        }

        return result;
    }
}