using System.Text.Json;

namespace Johwa.Common.Json;

public struct BooleanArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public BooleanArraySource(JsonElement booleanArrayProperty)
    {
        if (booleanArrayProperty.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(booleanArrayProperty)}는 배열이 아닙니다.");
        }
        Property = booleanArrayProperty;
    }
    public bool[] ToBooleanArray()
    {
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(Property)}는 배열이 아닙니다.");
        }

        bool[] booleanArray = new bool[Property.GetArrayLength()];

        for (int i = 0; i < booleanArray.Length; i++)
        {
            bool str = Property[i].GetBoolean();
            
            booleanArray[i] = str;
        }

        return booleanArray;
    }
    public bool this[int index] { get {
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(Property)}는 배열이 아닙니다.");
        }

        bool boolean = Property[index].GetBoolean();

        return boolean;
    } }
}