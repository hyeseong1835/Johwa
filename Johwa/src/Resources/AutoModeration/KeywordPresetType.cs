namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 자동 모더레이션 키워드 프리셋 타입 <br/>
/// preset keyword types used for AutoMod
/// </summary>
public enum KeywordPresetType
{
    /// <summary>
    /// 욕설이나 저주에 해당할 수 있는 단어들 <br/>
    /// words that may be considered forms of swearing or cursing
    /// </summary>
    Profanity = 1,

    /// <summary>
    /// 성적으로 노골적인 행동이나 활동을 지칭하는 단어들 <br/>
    /// words that refer to sexually explicit behavior or activity
    /// </summary>
    SexualContent = 2,

    /// <summary>
    /// 인신공격 또는 혐오 발언으로 간주될 수 있는 단어들 <br/>
    /// personal insults or words that may be considered hate speech
    /// </summary>
    Slurs = 3
}