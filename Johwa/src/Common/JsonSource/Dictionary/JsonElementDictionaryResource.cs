using System.Text.Json;

namespace Johwa.Common.JsonSource;

public struct JsonElementDictionaryResource
{
    public JsonElement Property { get; set; }

    public JsonElementDictionaryResource(JsonElement dictionaryProp)
    {
        if (dictionaryProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource는 Null일 수 없습니다.");
        }
        if (dictionaryProp.ValueKind != JsonValueKind.Object) {
            throw new InvalidOperationException($"{nameof(JsonElementDictionaryResource)}의 프로퍼티는 객체이어야 합니다.");
        }
        
        this.Property = dictionaryProp;
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