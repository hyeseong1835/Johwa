using System.Buffers;

namespace Johwa.Event.Data;

public interface IEventData
{
    public ReadOnlyMemory<byte> Container { get; }
    public int StartIndex { get; }
    public int Length { get; }
    public ReadOnlyMemory<byte> GetData()
    {
        return Container.Slice(StartIndex + StartIndex, Length);
    }
}
public struct EventData : IEventData
{
    ReadOnlyMemory<byte> IEventData.Container => container;
    int IEventData.StartIndex => startIndex;
    int IEventData.Length => length;

    public ReadOnlyMemory<byte> container;
    public int startIndex;
    public int length;

    public EventData(ReadOnlyMemory<byte> data, int startIndex, int length)
    {
        this.container = data;
        this.startIndex = startIndex;
        this.length = length;
    }

    public static implicit operator EventData(ReadOnlyMemory<byte> data)
    {
        return new EventData(data, 0, data.Length);
    }
    public static implicit operator ReadOnlyMemory<byte>(EventData data)
    {
        return data.container.Slice(data.startIndex, data.length);
    }
    public static implicit operator ReadOnlySequence<byte>(EventData data)
    {
        return new ReadOnlySequence<byte>(data.container.Slice(data.startIndex, data.length));
    }
}