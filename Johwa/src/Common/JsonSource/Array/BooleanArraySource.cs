using System.Text.Json;

namespace Johwa.Common.JsonSource;

/// <summary>
/// JsonElement (bool Array)를 사용하는 데 도움을 주는 래퍼입니다. <br/>
/// A wrapper that helps to use JsonElement (bool Array) <br/>
/// </summary>
public struct BooleanArraySource : IJsonSource
{
    /// <summary>
    /// 참조할 JsonElement <br/>
    /// .ValueKind가 Array이여야 합니다. <br/>
    /// <br/>
    /// .ValueKind must be Array <br/>
    /// JsonElement to reference <br/>
    /// </summary>
    public JsonElement Property { get; set; }

    public BooleanArraySource(JsonElement booleanArrayProp)
    {
        if (booleanArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource는 Null일 수 없습니다.");
        }
        if (booleanArrayProp.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(BooleanArraySource)}의 프로퍼티는 배열이어야 합니다.");
        }
        
        Property = booleanArrayProp;
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