using System.Text.Json;

namespace Johwa.Common.JsonSource;

/// <summary>
/// 
public struct IntArraySource : IJsonSource
{
    /// <summary>
    /// 참조할 JsonElement <br/>
    /// JsonElement to be referenced <br/>
    /// <br/>
    /// .ValueKind는 Null이어야 합니다. <br/>
    /// .ValueKind must be Null. <br/>
    /// </summary>
    public JsonElement Property { get; set; }

    /// <param name="intArrayProp">
    /// .ValueKind == Null -> ArgumentNullException: JsonSource cannot be null. <br/>
    /// .ValueKind != Array -> InvalidOperationException: The property of IntArraySource must be an array.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public IntArraySource(JsonElement intArrayProp)
    {
        if (intArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource cannot be null.");
        }
        if (intArrayProp.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(IntArraySource)}의 프로퍼티는 배열이어야 합니다.");
        }
        Property = intArrayProp;
    }
    public int this[int index]
        => Property[index].GetInt32();
}