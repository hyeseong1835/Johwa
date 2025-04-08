using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Utility;

public static class SnowflakeArraySourceUtility
{
    #region SnowflakeArraySource

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
        JsonElement prop;
        if (property.TryGetProperty(name, out prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"Null일 수 없는 프로퍼티 '{name}': null");
        }
        return new SnowflakeArraySource(prop);
    }

    #endregion


    #region NullableSnowflakeArraySource
    
    public static SnowflakeArraySource? GetNullableSnowflakeArraySource(this JsonElement property)
    {
        if (property.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new SnowflakeArraySource(property);
    }
    public static SnowflakeArraySource? FindNullableSnowflakeArraySource(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop) == false) {
            return null;
        }
        return new SnowflakeArraySource(prop);
    }
    public static SnowflakeArraySource? FindNullableSnowflakeArraySourceOrNull(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop) == false) {
            return null;
        }
        return new SnowflakeArraySource(prop);
    }

    #endregion
}