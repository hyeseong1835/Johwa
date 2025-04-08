using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Utility.Json;

public static class UlongArraySourceUtility
{
    public static UlongArraySource GetUlongArray(this JsonElement property)
    {
        return new UlongArraySource(property);
    }

    public static UlongArraySource FindUlongArraySource(this JsonElement property, string name)
    {
        return new UlongArraySource(property.GetProperty(name));
    }
    public static UlongArraySource? FindUlongArrayOrNull(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop)) {
            return null;
        }

        return new UlongArraySource(prop);
    }
}