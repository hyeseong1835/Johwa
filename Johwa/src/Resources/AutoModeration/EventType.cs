namespace Johwa.Resources.AutoModeration;

/// <summary>
/// 규칙이 검사되어야 하는 이벤트 종류 <br/>
/// Indicates in what event context a rule should be checked.
/// </summary>
public enum EventType
{
    /// <summary>
    /// 길드에서 사용자가 메시지를 보내거나 수정했을 때 <br/>
    /// when a member sends or edits a message in the guild
    /// </summary>
    MessageSend = 1,

    /// <summary>
    /// 길드에서 사용자가 프로필을 수정했을 때 <br/>
    /// when a member edits their profile
    /// </summary>
    MemberUpdate = 2
}