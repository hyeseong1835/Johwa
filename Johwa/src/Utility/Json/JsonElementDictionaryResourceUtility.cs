using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Utility;

public static class JsonElementDictionaryResourceUtility
{
    public static JsonElementDictionaryResource? FindJsonElementDictionaryOrNull(this JsonElement property, string propertyName)
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            return null;
        }
        return new JsonElementDictionaryResource(prop);
    }
}