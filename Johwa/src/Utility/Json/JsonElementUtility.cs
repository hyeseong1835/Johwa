using System.Text.Json;

namespace Johwa.Utility.Json;

public static class JsonElementUtility
{
    public static bool FindBoolean(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);

        return prop.GetBoolean();
    }
    public static bool? FindBooleanOrNull(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False) {
            return null;
        }

        return prop.GetBoolean();
    }
    public static bool FindBooleanOrTrue(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return true;
        }

        return prop.GetBoolean();
    }
    public static bool FindBooleanOrFalse(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return false;
        }

        return prop.GetBoolean();
    }
    public static int FindInt(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);

        return prop.GetInt32();
    }
    public static ulong? FindUlongOrNull(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.String) {
            if (ulong.TryParse(prop.GetString(), out ulong result)) {
                return result;
            }
            return null;
        }
        if (prop.ValueKind != JsonValueKind.Number) {
            return null;
        }
        return prop.GetUInt64();
    }

    public static string GetStringOrEmpty(this JsonElement property)
    {
        if (property.ValueKind != JsonValueKind.String) {
            return string.Empty;
        }
        
        string? result = property.GetString();
        if (result == null) {
            return string.Empty;
        }

        return result;
    }
    public static string FindString(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);

        return prop.GetString()!;
    }
    public static string FindStringOrEmpty(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return string.Empty;
        }
        
        if (prop.ValueKind != JsonValueKind.String) {
            return string.Empty;
        }

        string? result = prop.GetString();
        if (result == null) {
            return string.Empty;
        }
        
        return result;
    }
    public static string? FindStringOrNull(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return null;
        }
        return prop.GetString();
    }

    #region DateTime

    public static DateTime? GetDateTimeOrNull(this JsonElement property)
    {
        DateTime result;
        if (property.TryGetDateTime(out result) == false) {
            return null;
        }

        return result;
    }
    public static DateTime FindDateTime(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);

        return prop.GetDateTime();
    }
    public static DateTime? FindDateTimeOrNull(this JsonElement property, string propertyName)
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            return null;
        }

        DateTime result;
        if (prop.TryGetDateTime(out result) == false) {
            return null;
        }

        return result;
    }

    #endregion
}