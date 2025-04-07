using System.Text.Json;

namespace Johwa.Common.Json;

public struct JsonElementDictionaryResource
{
    public JsonElement Property { get; set; }

    public JsonElementDictionaryResource(JsonElement dictionaryProperty)
    {
        if (dictionaryProperty.ValueKind != JsonValueKind.Object) {
            throw new InvalidOperationException("Property는 객체가 아닙니다.");
        }
        
        this.Property = dictionaryProperty;
    }
    
    public JsonElement this[string key] {
        get {
            JsonElement.ObjectEnumerator enumerator = Property.EnumerateObject();
            while (enumerator.MoveNext()) {
                if (enumerator.Current.Name == key) {
                    return enumerator.Current.Value;
                }
            }
            throw new KeyNotFoundException($"키 '{key}'가 존재하지 않습니다.");
        }
    }
    public bool TryGetValue(string key, out JsonElement value)
    {
        JsonElement.ObjectEnumerator enumerator = Property.EnumerateObject();
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Name == key) {
                value = enumerator.Current.Value;
                return true;
            }
        }
        value = default;
        return false;
    }
}