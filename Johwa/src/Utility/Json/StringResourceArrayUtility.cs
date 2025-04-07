using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Utility.StringResourceArrayUtility;

public static class StringResourceArrayUtility
{
    public static StringArraySource GetStringResourceArray(this JsonElement property)
    {
        return new StringArraySource(property);
    }
    public static StringArraySource FindStringArraySource(this JsonElement property, string propertyName)
    {
        JsonElement prop = property.GetProperty(propertyName);
        return new StringArraySource(prop);
    }
    public static StringArraySource? FindStringResourceArrayOrNull(this JsonElement property, string propertyName)
    {
        if (property.TryGetProperty(propertyName, out JsonElement prop) == false) {
            return null;
        }
        return new StringArraySource(prop);
    }
}