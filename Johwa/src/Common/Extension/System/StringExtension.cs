using System.Runtime.InteropServices;

namespace Johwa.Common.Extension.System;

public static class StringExtension
{
    public static ReadOnlySpan<byte> AsByteSpan(this string str)
    {
        return MemoryMarshal.AsBytes(str.AsSpan());
    }
    public static void CopyToByteSpan(this string str, Span<byte> byteSpan)
    {
        str.AsByteSpan().CopyTo(byteSpan);
    }
    public static void CopyToByteMemory(this string str, Memory<byte> byteMemory)
    {
        str.AsByteSpan().CopyTo(byteMemory.Span);
    }
}