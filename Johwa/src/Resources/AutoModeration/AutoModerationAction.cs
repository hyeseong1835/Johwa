using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 자동 모더레이션 실행 작업 <br/>
/// An action which will execute whenever a rule is triggered.
/// </summary>
public struct AutoModerationAction : IJsonSource
{
    public JsonElement Property { get; set; }

    public AutoModerationAction(JsonElement autoModerationActionProperty)
    {
        this.Property = autoModerationActionProperty;
    }

    /// <summary>
    /// [ type ] <br/>
    /// 작업 유형 <br/>
    /// the type of action
    /// </summary>
    public int Type => Property.GetProperty("type").GetInt32();

    /// <summary>
    /// [ metadata ]? <br/>
    /// 실행 시 필요한 메타데이터 <br/>
    /// additional metadata needed during execution for this specific action type
    /// </summary>
    public ActionMetadata? Metadata { get {
        JsonElement prop;
        if (Property.TryGetProperty("metadata", out prop) == false)
            return null;

        if (prop.ValueKind == JsonValueKind.Null)
            return null;

        return new ActionMetadata(prop);
    } }
}