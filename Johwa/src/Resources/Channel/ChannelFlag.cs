namespace Johwa.Resources.Channel;

/// <summary>
/// 채널의 플래그를 나타냅니다. <br/>
/// Channel flags indicating various properties of a thread or media channel
/// </summary>
public enum ChannelFlag
{
    /// <summary>
    /// 해당 스레드가 상위 GUILD_FORUM 또는 GUILD_MEDIA 채널 상단에 고정됨 <br/>
    /// this thread is pinned to the top of its parent GUILD_FORUM or GUILD_MEDIA channel
    /// </summary>
    PINNED = 1 << 1,

    /// <summary>
    /// GUILD_FORUM 또는 GUILD_MEDIA 채널에서 스레드를 생성할 때 태그 지정이 필요함 <br/>
    /// whether a tag is required to be specified when creating a thread in a GUILD_FORUM or a GUILD_MEDIA channel
    /// </summary>
    REQUIRE_TAG = 1 << 4,

    /// <summary>
    /// 설정 시 임베디드 미디어 다운로드 옵션을 숨김 <br/>
    /// when set hides the embedded media download options. Available only for media channels
    /// </summary>
    HIDE_MEDIA_DOWNLOAD_OPTIONS = 1 << 15
}
