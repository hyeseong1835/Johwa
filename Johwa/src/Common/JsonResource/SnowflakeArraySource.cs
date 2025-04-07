using System.Text.Json;
using Johwa.Utility;

namespace Johwa.Common.Json;

public struct SnowflakeArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public SnowflakeArraySource(JsonElement intEnumArrayProperty)
    {
        if (intEnumArrayProperty.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(intEnumArrayProperty)}는 배열이 아닙니다.");
        }
        Property = intEnumArrayProperty;
    }
    public Snowflake this[int index]
        => Property[index].GetSnowflake();
        
    public Snowflake[] ToSnowflakeArray()
    {
        Snowflake[] snowflakeArray = new Snowflake[Property.GetArrayLength()];

        for (int i = 0; i < snowflakeArray.Length; i++)
        {
            Snowflake snowflake = Property[i].GetSnowflake();
            
            snowflakeArray[i] = snowflake;
        }

        return snowflakeArray;
    }
}