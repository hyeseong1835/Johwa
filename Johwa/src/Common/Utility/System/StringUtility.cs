namespace Johwa.Common.Utility.System;

public static class StringUtility
{
    public static Span<char> AsciiSpanToCharSpan(ReadOnlyMemory<byte> data)
    {
        Span<byte> dataByteSpan = stackalloc byte[data.Length];
        Span<char> charSpan = new char[data.Length];

        for (int i = 0; i < data.Length; i += 2)
        {
            charSpan[i] = (char)dataByteSpan[i];
        }
        return charSpan;
    }
    public static Span<char> CopyAsciiSpanToCharSpan(ReadOnlyMemory<byte> data, Span<char> charSpan)
    {
        Span<byte> dataByteSpan = stackalloc byte[data.Length];

        for (int i = 0; i < data.Length; i += 2)
        {
            charSpan[i] = (char)dataByteSpan[i];
        }
        return charSpan;
    }
}
