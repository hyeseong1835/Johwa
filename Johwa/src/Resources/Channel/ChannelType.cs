namespace Johwa.Resources.Channel;

/// <summary>
/// 채널 타입을 나타냅니다. <br/>
/// Type of the channel.
/// </summary>
public enum ChannelType
{
    /// <summary>
    /// 서버 내 텍스트 채널 <br/>
    /// a text channel within a server
    /// </summary>
    GuildText = 0,

    /// <summary>
    /// 사용자 간의 다이렉트 메시지 <br/>
    /// a direct message between users
    /// </summary>
    Dm = 1,

    /// <summary>
    /// 서버 내 음성 채널 <br/>
    /// a voice channel within a server
    /// </summary>
    GuildVoice = 2,

    /// <summary>
    /// 여러 사용자 간의 다이렉트 메시지 <br/>
    /// a direct message between multiple users
    /// </summary>
    GroupDm = 3,

    /// <summary>
    /// 최대 50개의 채널을 포함할 수 있는 분류 <br/>
    /// an organizational category that contains up to 50 channels
    /// </summary>
    GuildCategory = 4,

    /// <summary>
    /// 사용자가 팔로우하고 자신의 서버에 크로스포스트할 수 있는 채널 (예전 뉴스 채널) <br/>
    /// a channel that users can follow and crosspost into their own server (formerly news channels)
    /// </summary>
    GuildAnnouncement = 5,

    /// <summary>
    /// GUILD_ANNOUNCEMENT 채널 내 임시 하위 채널 <br/>
    /// a temporary sub-channel within a GUILD_ANNOUNCEMENT channel
    /// </summary>
    AnnouncementThread = 10,

    /// <summary>
    /// GUILD_TEXT 또는 GUILD_FORUM 채널 내 임시 하위 채널 <br/>
    /// a temporary sub-channel within a GUILD_TEXT or GUILD_FORUM channel
    /// </summary>
    PublicThread = 11,

    /// <summary>
    /// 초대받은 사용자와 MANAGE_THREADS 권한이 있는 사용자만 볼 수 있는 GUILD_TEXT 채널 내 임시 하위 채널 <br/>
    /// a temporary sub-channel within a GUILD_TEXT channel that is only viewable by those invited and those with the MANAGE_THREADS permission
    /// </summary>
    PrivateThread = 12,

    /// <summary>
    /// 청중과 함께 이벤트를 주최할 수 있는 음성 채널 <br/>
    /// a voice channel for hosting events with an audience
    /// </summary>
    GuildStageVoice = 13,

    /// <summary>
    /// 나열된 서버를 포함하는 허브 내 채널 <br/>
    /// the channel in a hub containing the listed servers
    /// </summary>
    GuildDirectory = 14,

    /// <summary>
    /// 스레드만 포함할 수 있는 채널 <br/>
    /// Channel that can only contain threads
    /// </summary>
    GuildForum = 15,

    /// <summary>
    /// GUILD_FORUM과 유사하며 스레드만 포함할 수 있는 채널 <br/>
    /// Channel that can only contain threads, similar to GUILD_FORUM channels
    /// </summary>
    GuildMedia = 16
}