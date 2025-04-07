using System.Text.Json;
using Johwa.Common.Json;

public static class JsonSourceArraySourceUtility
{
    public static JsonSourceArraySource<TJsonResource> GetJsonSourceArraySource<TJsonResource>(this JsonElement property)
        where TJsonResource : IJsonSource, new()
    {
        return new JsonSourceArraySource<TJsonResource>(property);
    }
    public static JsonSourceArraySource<TJsonResource> FindJsonSourceArraySource<TJsonResource>(this JsonElement property, string propertyName)
        where TJsonResource : IJsonSource, new()
    {
        JsonElement prop = property.GetProperty(propertyName);
        
        return new JsonSourceArraySource<TJsonResource>(prop);
    }
    public static JsonSourceArraySource<TJsonResource>? FindJsonSourceArraySourceOrNull<TJsonResource>(this JsonElement property, string propertyName)
        where TJsonResource : IJsonSource, new()
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            return null;
        }
        return new JsonSourceArraySource<TJsonResource>(prop);
    }
}