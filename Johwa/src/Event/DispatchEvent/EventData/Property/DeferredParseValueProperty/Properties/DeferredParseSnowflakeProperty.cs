using System.Text.Json;
using Johwa.Common;

namespace Johwa.Event.Data;

public class DeferredParseSnowflakeProperty : DeferredParseValueProperty<Snowflake>
{
    public DeferredParseSnowflakeProperty(
        ReadOnlyMemory<byte> data) : base(data) { }
        
    protected override Snowflake Parse()
    {
        return Snowflake.Parse(data.Span);
    }
}