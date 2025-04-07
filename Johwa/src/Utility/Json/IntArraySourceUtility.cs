using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Utility.Json;

public static class IntArraySourceUtility
{
    public static IntArraySource GetIntArray(this JsonElement property)
    {
        return new IntArraySource(property);
    }

    public static IntArraySource FindIntArraySource(this JsonElement property, string name)
    {
        return new IntArraySource(property.GetProperty(name));
    }
    public static IntArraySource? FindIntArrayOrNull(this JsonElement property, string name)
    {
        JsonElement prop;
        if (property.TryGetProperty(name, out prop)) {
            return null;
        }

        return new IntArraySource(prop);
    }
}