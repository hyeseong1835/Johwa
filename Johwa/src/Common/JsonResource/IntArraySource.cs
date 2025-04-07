using System.Text.Json;

namespace Johwa.Common.Json;

public struct IntArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public IntArraySource(JsonElement intEnumArrayProperty)
    {
        if (intEnumArrayProperty.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(intEnumArrayProperty)}는 배열이 아닙니다.");
        }
        Property = intEnumArrayProperty;
    }
    public int this[int index]
        => Property[index].GetInt32();
}