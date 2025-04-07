using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Utility;

public static class SnowflakeArraySourceUtility
{
    public static SnowflakeArraySource GetSnowflakeArraySource(this JsonElement property)
    {
        return new SnowflakeArraySource(property);
    }

    public static SnowflakeArraySource FindSnowflakeArraySource(this JsonElement property, string name)
    {
        return new SnowflakeArraySource(property.GetProperty(name));
    }
    
    public static SnowflakeArraySource? FindSnowflakeArraySourceOrNull(this JsonElement property, string name)
    {
        if (property.ValueKind != JsonValueKind.Array) {
            return null;
        }
        if (property.TryGetProperty(name, out JsonElement prop) == false) {
            return null;
        }

        return new SnowflakeArraySource(prop);
    }
}