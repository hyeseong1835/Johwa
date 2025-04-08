using System.Text.Json;

namespace Johwa.Common.JsonSource;

/// <summary>
/// JsonElement를 사용하는 데 도움을 주는 래퍼입니다. <br/>
/// A wrapper that helps to use JsonElement <br/>
/// </summary>
public interface IJsonSource
{
    /// <summary>
    /// 참조할 JsonElement <br/>
    /// JsonElement to be referenced <br/>
    /// </summary>
    JsonElement Property { get; set; }

    /// <summary>
    /// JsonElement을 래핑한 JsonSource를 생성합니다. <br/>
    /// JsonSource that wraps JsonElement is created.
    /// </summary>
    /// <typeparam name="T">
    /// JsonElement을 래핑할 JsonSource 타입입니다.
    /// The type of JsonSource that wraps JsonElement.
    /// </typeparam>
    /// <param name="prop">
    /// .ValueKind == JsonValueKind.Null <br/> 
    ///   -> ArgumentNullException: JsonSource cannot be null.
    /// </param>
    /// <returns>
    /// JsonElement를 래핑한 JsonResource <br/>
    /// JsonResource that wraps JsonElement
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// JsonSource는 Null일 수 없습니다. <br/>
    /// JsonSource cannot be null.
    /// </exception>
    public static T Create<T>(JsonElement prop) 
        where T : IJsonSource, new()
    {
        if (prop.ValueKind == JsonValueKind.Null) {
            // JsonSource는 Null일 수 없습니다.
            throw new ArgumentNullException($"JsonSource cannot be null.");
        }
        return new T() { Property = prop };
    }
}