using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.AuditLog;

/// <summary>
/// 개발중 <br/>
/// 감사 로그에서 개별 변경 사항을 나타냅니다. <br/>
/// Represents an individual change in the audit log.
/// </summary>
public struct AuditLogChangeObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public AuditLogChangeObject(JsonElement auditLogChangeProperty)
    {
        this.Property = auditLogChangeProperty;
    }

    /// <summary>
    /// [ new_value? ] <br/>
    /// 변경된 새 값 <br/>
    /// New value of the key
    /// </summary>

    /// <summary>
    /// [ old_value? ] <br/>
    /// 변경 전 이전 값 <br/>
    /// Old value of the key
    /// </summary>

    /// <summary>
    /// [ key ] <br/>
    /// 변경된 속성의 이름 <br/>
    /// Name of the changed entity, with a few exceptions
    /// </summary>
}
