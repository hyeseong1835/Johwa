using System.Text.Json;

namespace Johwa.Common.Json;

public struct StringArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public StringArraySource(JsonElement stringArrayProperty)
    {
        if (stringArrayProperty.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException("Property는 배열이 아닙니다.");
        }
        Property = stringArrayProperty;
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