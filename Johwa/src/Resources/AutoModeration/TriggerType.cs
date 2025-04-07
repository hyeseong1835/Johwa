using System.Text.Json.Serialization;

namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 규칙을 트리거할 수 있는 콘텐츠 유형을 식별합니다. <br/>
/// Characterizes the type of content which can trigger the rule.
/// </summary>
public enum TriggerType
{
    /// <summary>
    /// 사용자 정의 키워드 목록에 있는 단어를 포함하는 콘텐츠인지 확인 <br/>
    /// check if content contains words from a user defined list of keywords
    /// </summary>
    Keyword = 1,

    /// <summary>
    /// 일반적인 스팸 콘텐츠인지 확인 <br/>
    /// check if content represents generic spam
    /// </summary>
    Spam = 3,

    /// <summary>
    /// 내부에서 정의된 단어 세트에 포함된 단어를 포함하는 콘텐츠인지 확인 <br/>
    /// check if content contains words from internal pre-defined wordsets
    /// </summary>
    KeywordPreset = 4,

    /// <summary>
    /// 허용된 수보다 많은 고유 멘션을 포함하는 콘텐츠인지 확인 <br/>
    /// check if content contains more unique mentions than allowed
    /// </summary>
    MentionSpam = 5,

    /// <summary>
    /// 사용자 정의 키워드 목록에 있는 단어가 멤버 프로필에 포함되어 있는지 확인 <br/>
    /// check if member profile contains words from a user defined list of keywords
    /// </summary>
    MemberProfile = 6
}