using System.Text.Json;

namespace Johwa.Common.Utility;

public static class SnowflakeUtility
{
    #region Snowflake

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

    #endregion

    #region NullableSnowflake

    public static Snowflake? GetNullableSnowflake(this JsonElement property)
    {
        if (property.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new Snowflake(property.GetUInt64());
    }

    public static Snowflake? FindNullableSnowflake(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            throw new ArgumentNullException($"프로퍼티 '{propertyName}'를 찾을 수 없습니다.");
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new Snowflake(prop.GetUInt64());
    }
    public static Snowflake? FindNullableSnowflakeOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new Snowflake(prop.GetUInt64());
    }

    #endregion
}