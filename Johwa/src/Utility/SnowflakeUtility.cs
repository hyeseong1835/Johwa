using System.Text.Json;
using Johwa.Common;
namespace Johwa.Utility;

public static class SnowflakeUtility
{
    public static Snowflake GetSnowflake(this JsonElement property)
    {
        return new Snowflake(property.GetUInt64());
    }
    public static Snowflake FindSnowflake(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);

        return new Snowflake(prop.GetUInt64());
    }
    public static Snowflake? FindSnowflakeOrNull(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return null;
        }
        return new Snowflake(prop.GetUInt64());
    }
}