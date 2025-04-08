using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Utility;

public static class StringArraySourceUtility
{
    #region StringArraySource

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static StringArraySource GetStringArraySource(this JsonElement prop)
    {
        return new StringArraySource(prop);
    }
    public static StringArraySource FindStringArraySource(this JsonElement prop, string propertyName)
    {
        JsonElement findedProp = prop.GetProperty(propertyName);

        return new StringArraySource(findedProp);
    }
    public static StringArraySource? FindStringArraySourceOrNull(this JsonElement prop, string propertyName)
    {
        JsonElement findedProp;
        if (prop.TryGetProperty(propertyName, out findedProp) == false) {
            return null;
        }
        
        return new StringArraySource(findedProp);
    }

    #endregion


    #region NullableStringArraySource

    public static StringArraySource? GetNullableStringArraySource(this JsonElement prop)
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new StringArraySource(prop);
    }

    public static StringArraySource? FindNullableStringArraySource(this JsonElement prop, string propertyName)
    {
        JsonElement findedProp = prop.GetProperty(propertyName);

        if (findedProp.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new StringArraySource(findedProp);
    }
    public static StringArraySource? FindNullableStringArraySourceOrNull(this JsonElement prop, string propertyName)
    {
        JsonElement findedProp;
        if (prop.TryGetProperty(propertyName, out findedProp) == false) {
            return null;
        }

        if (findedProp.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return new StringArraySource(findedProp);
    }

    #endregion
}