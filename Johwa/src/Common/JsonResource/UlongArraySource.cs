using System.Text.Json;

namespace Johwa.Common.Json;

public struct UlongArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public UlongArraySource(JsonElement intEnumArrayProperty)
    {
        if (intEnumArrayProperty.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(intEnumArrayProperty)}는 배열이 아닙니다.");
        }
        Property = intEnumArrayProperty;
    }
    public ulong this[int index]
        => Property[index].GetUInt64();
}