using System.Text.Json;
using Johwa.Common;
using Johwa.Common.JsonSource;
using Johwa.Utility;
using Johwa.Utility.Json;

namespace Johwa.Resources.AuditLog;

/// <summary>
/// 개발중 <br/>
/// 감사 로그 항목 객체 <br/>
/// Each audit log entry represents a single administrative action (or event).
/// </summary>
public struct AuditLogEntryObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public AuditLogEntryObject(JsonElement auditLogEntryProperty)
    {
        this.Property = auditLogEntryProperty;
    }

    /// <summary>
    /// [ target_id? ] <br/>
    /// 영향을 받은 엔티티의 ID (웹훅, 유저, 역할 등) <br/>
    /// ID of the affected entity (webhook, user, role, etc.)
    /// </summary>
    public string? TargetId
        => Property.FindStringOrNull("target_id");

    /// <summary>
    /// [ changes? ] <br/>
    /// 대상 엔티티에 적용된 변경 사항들 <br/>
    /// Changes made to the target_id
    /// </summary>
    public JsonSourceArraySource<AuditLogChangeObject>? Changes
        => Property.FindJsonSourceArraySourceOrNull<AuditLogChangeObject>("changes");

    /// <summary>
    /// [ user_id? ] <br/>
    /// 변경을 수행한 사용자 또는 앱의 ID <br/>
    /// User or app that made the changes
    /// </summary>
    public Snowflake? UserId
        => Property.FindSnowflakeOrNull("user_id");

    /// <summary>
    /// [ id ] <br/>
    /// 감사 로그 항목 ID <br/>
    /// ID of the entry
    /// </summary>
    public Snowflake Id 
        => Property.GetProperty("id").GetSnowflake();

    /// <summary>
    /// [ action_type ] <br/>
    /// 발생한 액션의 타입 <br/>
    /// Type of action that occurred
    /// </summary>

    /// <summary>
    /// [ options? ] <br/>
    /// 특정 이벤트 타입에 대한 추가 정보 <br/>
    /// Additional info for certain event types
    /// </summary>

    /// <summary>
    /// [ reason? ] <br/>
    /// 변경 사유 (1-512자) <br/>
    /// Reason for the change (1-512 characters)
    /// </summary>
    public string? Reason
        => Property.FindStringOrNull("reason");
}
