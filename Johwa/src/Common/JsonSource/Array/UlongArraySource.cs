using System.Text.Json;

namespace Johwa.Common.JsonSource;

public struct UlongArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public UlongArraySource(JsonElement ulongArrayProp)
    {
        if (ulongArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource는 Null일 수 없습니다.");
        }
        if (ulongArrayProp.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(UlongArraySource)}의 프로퍼티는 배열이어야 합니다.");
        }
        Property = ulongArrayProp;
    }
    public ulong this[int index]
        => Property[index].GetUInt64();
}