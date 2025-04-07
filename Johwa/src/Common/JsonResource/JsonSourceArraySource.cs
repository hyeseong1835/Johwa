using System.Text.Json;

namespace Johwa.Common.Json;

public struct JsonSourceArraySource<TJsonSource> : IJsonSource
    where TJsonSource : IJsonSource, new()
{
    public JsonElement Property { get; set; }

    public JsonSourceArraySource(JsonElement arrayProperty)
    {
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException("Property는 배열이 아닙니다.");
        }
        
        this.Property = arrayProperty;
    }

    public int Count => Property.GetArrayLength();
    
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
}