namespace Johwa.Core;

/// <summary>
/// Discord 채널 유형 정의
/// </summary>
public enum ChannelType
{
    /// <summary> 
    /// 
    /// 서버 내 텍스트 채널 
    /// </summary>
    GUILD_TEXT = 0,

    /// <summary> 사용자 간 1:1 DM </summary>
    DM = 1,

    /// <summary> 서버 내 음성 채널 </summary>
    GUILD_VOICE = 2,

    /// <summary> 다수 사용자 간 그룹 DM </summary>
    GROUP_DM = 3,

    /// <summary> 최대 50개 채널을 포함할 수 있는 카테고리 </summary>
    GUILD_CATEGORY = 4,

    /// <summary> 사용자가 팔로우하고 자신의 서버에 교차 게시할 수 있는 공지 채널 (이전: 뉴스 채널) </summary>
    GUILD_ANNOUNCEMENT = 5,

    /// <summary> GUILD_ANNOUNCEMENT 내의 임시 서브 채널 (스레드) </summary>
    ANNOUNCEMENT_THREAD = 10,

    /// <summary> GUILD_TEXT 또는 GUILD_FORUM 내의 공개 스레드 </summary>
    PUBLIC_THREAD = 11,

    /// <summary> GUILD_TEXT 내의 비공개 스레드 (초대받은 사용자와 관리자만 접근 가능) </summary>
    PRIVATE_THREAD = 12,

    /// <summary> 청중과 함께 이벤트를 개최하는 무대 채널 </summary>
    GUILD_STAGE_VOICE = 13,

    /// <summary> 허브에서 서버 목록을 포함하는 디렉토리 채널 </summary>
    GUILD_DIRECTORY = 14,

    /// <summary> 오직 스레드만 포함할 수 있는 포럼 채널 </summary>
    GUILD_FORUM = 15,

    /// <summary> GUILD_FORUM과 유사하지만 미디어 중심인 스레드 전용 채널 (개발 중 기능) </summary>
    GUILD_MEDIA = 16
}