using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Utility.Json;

public static class IJsonSourceUtility
{
    public static IJsonSource GetJsonSource<TJsonSource>(this JsonElement property)
        where TJsonSource : IJsonSource, new()
    {
        return IJsonSource.Create<TJsonSource>(property);
    }
    public static IJsonSource FindJsonSource<TJsonSource>(this JsonElement property, string propertyName)
        where TJsonSource : IJsonSource, new()
    {
        JsonElement prop = property.GetProperty(propertyName);
        
        return IJsonSource.Create<TJsonSource>(prop);
    }
    public static TJsonSource? FindJsonSourceOrNull<TJsonSource>(this JsonElement property, string propertyName)
        where TJsonSource : struct, IJsonSource
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            return null;
        }
        return IJsonSource.Create<TJsonSource>(prop);
    }
}