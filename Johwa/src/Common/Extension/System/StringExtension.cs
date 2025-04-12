using System.Runtime.InteropServices;

namespace Johwa.Common.Extension.System;

public static class StringExtension
{
    public static ReadOnlySpan<byte> AsByteSpan(this string str)
    {
        return MemoryMarshal.AsBytes(str.AsSpan());
    }
}