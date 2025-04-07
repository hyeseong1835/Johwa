using System.Text.Json;
using Johwa.Common.Json;
using Johwa.Resources.AutoModeration;

namespace Johwa.Resources.AuditLog;

/// <summary>
/// 개발중 <br/>
/// 감사 로그 전체 데이터를 나타냅니다. <br/>
/// Represents the audit log for a guild, including entries and referenced objects.
/// </summary>
public struct AuditLogObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public AuditLogObject(JsonElement auditLogProperty)
    {
        this.Property = auditLogProperty;
    }

    /// <summary>
    /// [ application_commands ] <br/>
    /// 감사 로그에서 참조된 애플리케이션 커맨드 목록 <br/>
    /// List of application commands referenced in the audit log
    /// </summary>
    
    /// <summary>
    /// [ audit_log_entries ] <br/>
    /// 감사 로그 항목 목록 <br/>
    /// List of audit log entries, sorted from most to least recent
    /// </summary>
    public JsonSourceArraySource<AuditLogEntryObject> AuditLogEntries
        => Property.GetProperty("audit_log_entries").GetJsonSourceArraySource<AuditLogEntryObject>();

    /// <summary>
    /// [ auto_moderation_rules ] <br/>
    /// 감사 로그에서 참조된 자동 모더레이션 규칙 목록 <br/>
    /// List of auto moderation rules referenced in the audit log
    /// </summary>
    public JsonSourceArraySource<AutoModerationRuleObject> AutoModerationRules 
        => Property.GetProperty("auto_moderation_rules").GetJsonSourceArraySource<AutoModerationRuleObject>();

    /// <summary>
    /// [ guild_scheduled_events ] <br/>
    /// 감사 로그에서 참조된 예약된 이벤트 목록 <br/>
    /// List of guild scheduled events referenced in the audit log
    /// </summary>

    /// <summary>
    /// [ integrations ] <br/>
    /// 감사 로그에서 참조된 통합 정보 목록 <br/>
    /// List of partial integration objects
    /// </summary>

    /// <summary>
    /// [ threads ] <br/>
    /// 감사 로그에서 참조된 쓰레드 채널 목록 <br/>
    /// List of threads referenced in the audit log
    /// </summary>

    /// <summary>
    /// [ users ] <br/>
    /// 감사 로그에서 참조된 사용자 목록 <br/>
    /// List of users referenced in the audit log
    /// </summary>

    /// <summary>
    /// [ webhooks ] <br/>
    /// 감사 로그에서 참조된 웹후크 목록 <br/>
    /// List of webhooks referenced in the audit log
    /// </summary>
}
