using System.Text.Json;

namespace Johwa.Common.Json;

public interface IJsonSource
{
    /// <summary>
    /// JSON 속성의 값을 가져옵니다. <br/>
    /// Gets the value of the JSON property.
    /// </summary>
    JsonElement Property { get; set; }

    public static T Create<T>(JsonElement property) 
        where T : IJsonSource, new()
    {
        return new T() { Property = property };
    }
}