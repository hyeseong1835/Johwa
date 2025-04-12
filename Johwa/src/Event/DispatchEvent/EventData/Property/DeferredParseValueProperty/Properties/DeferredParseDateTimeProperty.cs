using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseDateTimeProperty : DeferredParseValueProperty<DateTime>
{
    public DeferredParseDateTimeProperty(
        ReadOnlyMemory<byte> data) : base(data) { }
        
    protected override DateTime Parse()
    {
        Span<char> charSpan = stackalloc char[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            charSpan[i] = (char)data.Span[i];
        }
        return DateTime.Parse(charSpan);
    }
}