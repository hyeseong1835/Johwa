using System.Buffers;

namespace Johwa.Extension.System;
public static class ReadOnlyMemoryExtension
{
    public static ReadOnlySequence<byte> ToReadOnlySequence(this ReadOnlyMemory<byte> memory)
    {
        return new ReadOnlySequence<byte>(memory);
    }
}