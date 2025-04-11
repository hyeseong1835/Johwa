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

    public EventData(IEventData data)
    {
        this.container = data.Container;
        this.startIndex = data.StartIndex;
        this.length = data.Length;
    }
    public EventData(IEventData container, int startIndex, int length)
    {
        this.container = container.GetData();
        this.startIndex = startIndex;
        this.length = length;
    }
    public EventData(ReadOnlyMemory<byte> container, int startIndex, int length)
    {
        this.container = container;
        this.startIndex = startIndex;
        this.length = length;
    }

    public static implicit operator EventData(ReadOnlyMemory<byte> data)
    {
        return new EventData(data, 0, data.Length);
    }
    public static implicit operator ReadOnlyMemory<byte>(EventData data)
    {
        return data.GetData();
    }
    public static implicit operator ReadOnlySequence<byte>(EventData data)
    {
        return new ReadOnlySequence<byte>(data.GetData());
    }
    public ReadOnlyMemory<byte> GetData()
    {
        return container.Slice(startIndex, length);
    }
}