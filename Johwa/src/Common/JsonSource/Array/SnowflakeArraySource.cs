using System.Text.Json;
using Johwa.Utility;

namespace Johwa.Common.JsonSource;

public struct SnowflakeArraySource : IJsonSource
{
    public JsonElement Property { get; set; }

    public SnowflakeArraySource(JsonElement ulongArrayProp)
    {
        if (ulongArrayProp.ValueKind == JsonValueKind.Null) {
            throw new ArgumentNullException($"JsonSource는 Null일 수 없습니다.");
        }
        if (ulongArrayProp.ValueKind != JsonValueKind.Array) {
            throw new InvalidOperationException($"{nameof(SnowflakeArraySource)}의 프로퍼티는 배열이어야 합니다.");
        }
        Property = ulongArrayProp;
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