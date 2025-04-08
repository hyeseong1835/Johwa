using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Utility.Json;

public static class IntArraySourceUtility
{
    #region IntArraySource

    public static IntArraySource GetIntArray(this JsonElement property)
    {
        return new IntArraySource(property);
    }

    public static IntArraySource FindIntArraySource(this JsonElement property, string name)
    {
        JsonElement prop = property.GetProperty(name);
        return new IntArraySource(prop);
    }
    public static IntArraySource? FindIntArrayOrNull(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop)) {
            return null;
        }

        return new IntArraySource(prop);
    }

    #endregion


    #region NullableIntArraySource

    public static IntArraySource? GetNullableIntArray(this JsonElement property)
    {
        if (property.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new IntArraySource(property);
    }

    public static IntArraySource? FindNullableIntArraySource(this JsonElement property, string name)
    {
        JsonElement prop = property.GetProperty(name);
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new IntArraySource(prop);
    }
    public static IntArraySource? FindNullableIntArrayOrNull(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop) == false) {
            return null;
        }

        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }

        return new IntArraySource(prop);
    }
    #endregion
}