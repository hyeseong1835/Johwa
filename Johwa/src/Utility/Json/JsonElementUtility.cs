using System.Text.Json;

namespace Johwa.Utility.Json;

public static class JsonElementUtility
{
    #region Boolean

    public static bool GetBoolean(this JsonElement prop)
    {
        return prop.GetBoolean();
    }
    public static bool FindBoolean(this JsonElement data, string propName)
    {
        JsonElement prop = data.GetProperty(propName);

        return prop.GetBoolean();
    }
    public static bool? FindBooleanOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False) {
            return null;
        }

        return prop.GetBoolean();
    }
    public static bool FindBooleanOrTrue(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return true;
        }

        return prop.GetBoolean();
    }
    public static bool FindBooleanOrFalse(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return false;
        }

        return prop.GetBoolean();
    }

    #endregion

    #region NullableBoolean

    public static bool? GetNullableBoolean(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False) {
            throw new ArgumentException($"'{prop.ValueKind}'는 Bool이 아닙니다.");
        }

        return prop.GetBoolean();
    }
    public static bool? FindNullableBoolean(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False) {
            throw new ArgumentException($"'{prop.ValueKind}'는 Bool이 아닙니다.");
        }

        return prop.GetBoolean();
    }
    public static bool? FindNullableBooleanOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False) {
            return null;
        }

        return prop.GetBoolean();
    }

    #endregion


    #region Int

    public static int GetInt(this JsonElement prop)
    {
        return prop.GetInt32();
    }
    public static int FindInt(this JsonElement data, string propName)
    {
        JsonElement prop = data.GetProperty(propName);

        return prop.GetInt32();
    }
    public static int? FindIntOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        
        return prop.GetInt32();
    }

    #endregion

    #region NullableInt

    public static int? GetNullableInt(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetInt32();
    }
    public static int? FindNullableInt(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            throw new ArgumentException($"프로퍼티 '{propName}'를 찾을 수 없습니다.");
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetInt32();
    }
    public static int? FindNullableIntOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetInt32();
    }

    #endregion


    #region Ulong

    public static ulong GetUlong(this JsonElement prop)
    {
        return prop.GetUInt64();
    }
    public static ulong FindUlong(this JsonElement data, string propName)
    {
        JsonElement prop = data.GetProperty(propName);

        return prop.GetUInt64();
    }
    public static ulong? FindUlongOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }

        return prop.GetUInt64();
    }

    #endregion

    #region NullableUlong

    public static ulong? GetNullableUlong(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetUInt64();
    }
    public static ulong? FindNullableUlong(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            throw new ArgumentException($"프로퍼티 '{propName}'를 찾을 수 없습니다.");
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetUInt64();
    }
    public static ulong? FindNullableUlongOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetUInt64();
    }

    #endregion


    #region String
    
    public static string GetString(this JsonElement prop)
    {
        string? result = prop.GetString();
        
        return result!;
    }
    public static string FindString(this JsonElement data, string propName)
    {
        JsonElement prop = data.GetProperty(propName);

        return prop.GetString()!;
    }
    public static string? FindStringOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        return prop.GetString();
    }

    #endregion

    #region NullableString

    public static string? GetNullableString(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetString();
    }
    public static string? FindNullableString(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            throw new ArgumentException($"프로퍼티 '{propName}'를 찾을 수 없습니다.");
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetString();
    }
    public static string? FindNullableStringOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetString();
    }

    #endregion


    #region DateTime

    public static DateTime FindDateTime(this JsonElement data, string propName)
    {
        JsonElement findedProp = data.GetProperty(propName);

        return findedProp.GetDateTime();
    }
    public static DateTime? FindDateTimeOrNull(this JsonElement data, string propName)
    {
        JsonElement prop;
        if (data.TryGetProperty(propName, out prop) == false) {
            return null;
        }

        return prop.GetDateTime();
    }

    #endregion

    #region NullableDateTime

    public static DateTime? GetNullableDateTime(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetDateTime();
    }
    public static DateTime? FindNullableDateTime(this JsonElement data, string propName)
    {
        JsonElement prop = data.GetProperty(propName);
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetDateTime();
    }
    public static DateTime? FindNullableDateTimeOrNull(this JsonElement data, string propName)
    {
        if (data.TryGetProperty(propName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return prop.GetDateTime();
    }

    #endregion
}