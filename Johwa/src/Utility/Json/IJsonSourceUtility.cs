using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Utility.Json;

public static class IJsonSourceUtility
{
    #region JsonSource

    public static TJsonSource GetJsonSource<TJsonSource>(this JsonElement property)
        where TJsonSource : IJsonSource, new()
    {
        return IJsonSource.Create<TJsonSource>(property);
    }

    public static TJsonSource FindJsonSource<TJsonSource>(this JsonElement property, string propertyName)
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

    #endregion


    #region NullableJsonSource

    public static TJsonSource GetNullableJsonSource<TJsonSource>(this JsonElement property)
        where TJsonSource : struct, IJsonSource
    {
        return IJsonSource.Create<TJsonSource>(property);
    }
    public static TJsonSource? GetNullableJsonSourceOrNull<TJsonSource>(this JsonElement property)
        where TJsonSource : struct, IJsonSource
    {
        if (property.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return IJsonSource.Create<TJsonSource>(property);
    }

    public static TJsonSource? FindNullableJsonSource<TJsonSource>(this JsonElement property, string propertyName)
        where TJsonSource : struct, IJsonSource
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            throw new ArgumentNullException($"프로퍼티 '{propertyName}'를 찾을 수 없습니다.");
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return IJsonSource.Create<TJsonSource>(prop);
    }
    public static TJsonSource? FindNullableJsonSourceOrNull<TJsonSource>(this JsonElement property, string propertyName)
        where TJsonSource : struct, IJsonSource
    {
        JsonElement prop;
        if (property.TryGetProperty(propertyName, out prop) == false) {
            return null;
        }
        if (prop.ValueKind == JsonValueKind.Null) {
            return null;
        }
        return IJsonSource.Create<TJsonSource>(prop);
    }

    #endregion
}