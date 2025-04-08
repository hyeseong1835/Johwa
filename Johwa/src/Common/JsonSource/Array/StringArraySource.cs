using System.Text.Json;

namespace Johwa.Common.JsonSource;

public struct StringArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    /// <param name="stringArrayProp">
    /// .ValueKind == JsonValueKind.Null -> ArgumentNullException <br/>
    /// .ValueKind != JsonValueKind.Array -> InvalidOperationException <br/>
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public StringArraySource(JsonElement stringArrayProp)
    {
        if (stringArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource는 Null일 수 없습니다.");
        }
        if (stringArrayProp.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(StringArraySource)}의 프로퍼티는 배열이어야 합니다.");
        }
        Property = stringArrayProp;
    }
    public string[] ToStringArray()
    {
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException("Property is not an array.");
        }

        string[] strArray = new string[Property.GetArrayLength()];

        for (int i = 0; i < strArray.Length; i++)
        {
            string? str = Property[i].GetString();
            if (str == null)
            {
                strArray[i] = string.Empty;
            }
            else
            {
                strArray[i] = str;
            }
        }

        return strArray;
    }
    public string this[int index] { get {
        if (Property.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException("Property is not an array.");
        }

        string? str = Property[index].GetString();
        if (str == null) {
            return string.Empty;
        }
        
        return str;
    } }
}