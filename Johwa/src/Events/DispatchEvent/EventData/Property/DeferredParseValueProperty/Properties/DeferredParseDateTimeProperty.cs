using System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseDateTimeProperty : DeferredParseValueProperty<DateTime>
{
    public DeferredParseDateTimeProperty(
        ReadOnlyMemory<byte> data) : base(data) { }
        
    protected override DateTime Parse()
    {
        return DateTime.Parse(data.Span);
    }
}