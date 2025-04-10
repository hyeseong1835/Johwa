namespace Johwa.Event.Data;

public interface IEventDataObject
{
    public ReadOnlySpan<byte> Data { get; }
}

public class EventDataObjectUnit
{
    protected IEventDataObject jsonObject;
    protected int startIndex;
    protected int length;

    public EventDataObjectUnit(IEventDataObject jsonObject, int startIndex, int length)
    {
        this.jsonObject = jsonObject;
        this.startIndex = startIndex;
        this.length = length;
    }

    public ReadOnlySpan<byte> SliceData()
        => jsonObject.Data.Slice(startIndex, length);
}