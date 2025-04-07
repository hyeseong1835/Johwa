using System.Text.Json;
using Johwa.Common;
using Johwa.Common.Json;

namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 개발중 <br/>
/// 자동 모더레이션 실행 메타데이터 <br/>
/// Additional data used when an action is executed.
/// </summary>
public struct ActionMetadata : IJsonSource
{
    public JsonElement Property { get; set; }

    public ActionMetadata(JsonElement actionMetadataProperty)
    {
        this.Property = actionMetadataProperty;
    }

    /// <summary>
    /// [ channel_id ]? <br/>
    /// 로그를 기록할 채널 ID <br/>
    /// channel to which user content should be logged
    /// </summary>
    public Snowflake? ChannelId { get {
        JsonElement prop;
        if (Property.TryGetProperty("channel_id", out prop) == false)
            return null;

        return prop.GetUInt64();
    } }

    /// <summary>
    /// [ duration_seconds ]? <br/>
    /// 타임아웃 시간(초) <br/>
    /// timeout duration in seconds
    /// </summary>
    public int? DurationSeconds { get {
        JsonElement prop;
        if (Property.TryGetProperty("duration_seconds", out prop) == false)
            return null;

        return prop.GetInt32();
    } }

    /// <summary>
    /// [ custom_message ]? <br/>
    /// 차단 메시지에 표시될 추가 설명 <br/>
    /// additional explanation that will be shown to members whenever their message is blocked
    /// </summary>
    public string? CustomMessage { get {
        JsonElement prop;
        if (Property.TryGetProperty("custom_message", out prop) == false)
            return null;

        return prop.GetString();
    } }
}
