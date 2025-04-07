using System.Text.Json;
using Johwa.Common;
using Johwa.Common.Json;
using Johwa.Utility;
using Johwa.Utility.Json;

namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 자동 모더레이션 규칙 객체 <br/>
/// Auto Moderation Rule Object
/// </summary>
public struct AutoModerationRuleObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public AutoModerationRuleObject(JsonElement autoModerationRuleProperty)
    {
        this.Property = autoModerationRuleProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 이 규칙의 ID <br/>
    /// the id of this rule
    /// </summary>
    public Snowflake Id 
        => Property.FindSnowflake("id");

    /// <summary>
    /// [ guild_id ] <br/>
    /// 이 규칙이 속한 길드의 ID <br/>
    /// the id of the guild which this rule belongs to
    /// </summary>
    public Snowflake GuildId 
        => Property.FindSnowflake("guild_id");

    /// <summary>
    /// [ name ] <br/>
    /// 규칙 이름 <br/>
    /// the rule name
    /// </summary>
    public string Name 
        => Property.FindString("name");

    /// <summary>
    /// [ creator_id ] <br/>
    /// 이 규칙을 처음 생성한 사용자 <br/>
    /// the user which first created this rule
    /// </summary>
    public Snowflake CreatorId 
        => Property.FindSnowflake("creator_id");

    /// <summary>
    /// [ event_type ] <br/>
    /// 규칙 이벤트 유형 <br/>
    /// the rule event type
    /// </summary>
    public EventType EventType 
        => (EventType)Property.FindInt("event_type");

    /// <summary>
    /// [ trigger_type ] <br/>
    /// 규칙 트리거 유형 <br/>
    /// the rule trigger type
    /// </summary>
    public TriggerType TriggerType 
        => (TriggerType)Property.FindInt("trigger_type");

    /// <summary>
    /// [ trigger_metadata ] <br/>
    /// 규칙 트리거 메타데이터 <br/>
    /// the rule trigger metadata
    /// </summary>
    public TriggerMetadata TriggerMetadata 
        => new TriggerMetadata(Property.GetProperty("trigger_metadata"));

    /// <summary>
    /// [ actions ] <br/>
    /// 규칙이 트리거되었을 때 실행될 작업 목록 <br/>
    /// the actions which will execute when the rule is triggered
    /// </summary>
    public JsonSourceArraySource<AutoModerationAction> Actions 
        => Property.FindJsonSourceArraySource<AutoModerationAction>("actions");

    /// <summary>
    /// [ enabled ] <br/>
    /// 규칙 활성화 여부 <br/>
    /// whether the rule is enabled
    /// </summary>
    public bool Enabled 
        => Property.FindBoolean("enabled");

    /// <summary>
    /// [ exempt_roles ] <br/>
    /// 규칙의 영향을 받지 않는 역할 ID 목록 <br/>
    /// the role ids that should not be affected by the rule (Maximum of 20)
    /// </summary>
    public SnowflakeArraySource ExemptRoles 
        => Property.FindSnowflakeArraySource("exempt_roles");

    /// <summary>
    /// [ exempt_channels ] <br/>
    /// 규칙의 영향을 받지 않는 채널 ID 목록 <br/>
    /// the channel ids that should not be affected by the rule (Maximum of 50)
    /// </summary>
    public SnowflakeArraySource ExemptChannels 
        => Property.FindSnowflakeArraySource("exempt_channels");
}
