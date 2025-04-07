using System.Text.Json;
using Johwa.Common.Json;
using Johwa.Utility.Json;
using Johwa.Utility.StringResourceArrayUtility;

namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 규칙이 트리거되는지를 판단하기 위한 추가 데이터 <br/>
/// Additional data used to determine whether a rule should be triggered
/// </summary>
public struct TriggerMetadata : IJsonSource
{
    public JsonElement Property { get; set; }

    public TriggerMetadata(JsonElement triggerMetadataProperty)
    {
        Property = triggerMetadataProperty;
    }

    /// <summary>
    /// [ keyword_filter ] <br/>
    /// 콘텐츠에서 검색될 문자열 목록 (최대 1000개) <br/>
    /// substrings which will be searched for in content (Maximum of 1000)
    /// </summary>
    public StringArraySource KeywordFilter 
        => Property.FindStringArraySource("keyword_filter");

    /// <summary>
    /// [ regex_patterns ] <br/>
    /// 콘텐츠에서 매칭될 정규 표현식 목록 (최대 10개) <br/>
    /// regular expression patterns which will be matched against content (Maximum of 10)
    /// </summary>
    public StringArraySource RegexPatterns 
        => Property.FindStringArraySource("regex_patterns");

    /// <summary>
    /// [ presets ] <br/>
    /// 콘텐츠에서 검색될 내부 사전 정의 키워드 세트 목록 <br/>
    /// the internally pre-defined wordsets which will be searched for in content
    /// </summary>
    public IntArraySource Presets 
        => Property.FindIntArraySource("presets");

    /// <summary>
    /// [ allow_list ] <br/>
    /// 트리거되지 않아야 하는 문자열 목록 (최대 100 또는 1000개) <br/>
    /// substrings which should not trigger the rule (Maximum of 100 or 1000)
    /// </summary>
    public StringArraySource AllowList 
        => Property.FindStringArraySource("allow_list");

    /// <summary>
    /// [ mention_total_limit ] <br/>
    /// 메시지당 허용되는 고유 멘션 수 (최대 50) <br/>
    /// total number of unique role and user mentions allowed per message (Maximum of 50)
    /// </summary>
    public int MentionTotalLimit 
        => Property.FindInt("mention_total_limit");

    /// <summary>
    /// [ mention_raid_protection_enabled ] <br/>
    /// 멘션 공격을 자동으로 감지할지 여부 <br/>
    /// whether to automatically detect mention raids
    /// </summary>
    public bool MentionRaidProtectionEnabled
        => Property.FindBoolean("mention_raid_protection_enabled");
}
