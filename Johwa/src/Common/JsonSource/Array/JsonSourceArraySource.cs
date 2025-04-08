using System.Text.Json;

namespace Johwa.Common.JsonSource;

public struct JsonSourceArraySource<TJsonSource> : IJsonSource
    where TJsonSource : IJsonSource, new()
{
    public JsonElement Property { get; set; }

    public JsonSourceArraySource(JsonElement jsonSourceArrayProp)
    {
        if (jsonSourceArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException("JsonSource는 Null일 수 없습니다.");
        }
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(JsonSourceArraySource<TJsonSource>)}의 Property는 배열이어야 합니다.");
        }
        
        this.Property = jsonSourceArrayProp;
    }

    public int Length => Property.GetArrayLength();
    
    public TJsonSource this[int index] 
        => IJsonSource.Create<TJsonSource>(Property[index]);
    
    public TJsonSource[] ToArray()
    {
        TJsonSource[] array = new TJsonSource[Property.GetArrayLength()];

        for (int i = 0; i < array.Length; i++)
        {
            TJsonSource resource = IJsonSource.Create<TJsonSource>(Property[i]);
            array[i] = resource;
        }

        return array;
    }
    public IEnumerable<TJsonSource> ToEnumerable()
    {
        int length = Property.GetArrayLength();

        for (int i = 0; i < length; i++)
        {
            yield return IJsonSource.Create<TJsonSource>(Property[i]);
        }
    }
}