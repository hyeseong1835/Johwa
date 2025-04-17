using System.Text.Json;
using Johwa.Event.Data;
using Johwa.Common.Debug;
using Johwa.Common.Extension.System.Text.Json;
using Johwa.Common;

namespace Johwa.Event.DispatchEvents;

[DispatchEvent(DispatchEventType.GUILD_CREATE)]
public class GuildCreateEvent : DispatchEvent
{
    public override void Handle(DiscordGatewayClient client, ReadOnlyMemory<byte> dataMemory)
    {
        // 핸들 컨텍스트 얻기
        EventHandleContext<AvailableGuildCreateEventData>? availableEventDataContext = EventHandler<AvailableGuildCreateEventData>.GetContext(client);
        EventHandleContext<UnavailableGuildCreateEventData>? unavailableEventDataContext = EventHandler<UnavailableGuildCreateEventData>.GetContext(client);
        
        // 핸들러 보유 여부 : 컨텍스트가 존재하고 활성화된 핸들러의 개수가 1 이상이면 true
        bool hasEnabledAvailableEventData = availableEventDataContext != null && availableEventDataContext.enabledHandlerCount > 0;
        bool hasEnabledUnavailableEventData = unavailableEventDataContext != null && unavailableEventDataContext.enabledHandlerCount > 0;

        // 핸들러가 없으면 종료
        if (hasEnabledAvailableEventData == false 
            && hasEnabledUnavailableEventData == false) return;

        // Json 읽기
        ReadOnlySpan<byte> dataSpan = dataMemory.Span;
        Utf8JsonReader reader = new(dataSpan);

        // unavailable 찾기
        if (reader.TryFindPropertyName("unavailable") == false) {
            JohwaLogger.Log(LogSeverity.Error, "GuildCreateEvent의 데이터에 \"unavailable\" 속성이 존재하지 않습니다.");
            return;
        }
        
        // 값으로 이동
        reader.Read();

        switch (reader.TokenType)
        {
            case JsonTokenType.True: {
                AvailableGuildCreateEventData eventData = new(dataMemory);
                EventHandler<AvailableGuildCreateEventData>.OnHandled(client, eventData);
                break;
            }
            case JsonTokenType.False: {
                UnavailableGuildCreateEventData eventData = new(dataMemory);
                EventHandler<UnavailableGuildCreateEventData>.OnHandled(client, eventData);
                break;
            }
            default: {
                JohwaLogger.Log(LogSeverity.Error, "값 \"unavailable\"이 잘못되었습니다.");
                break;
            }
        }
        return;
    }
}

public abstract class GuildCreateEventData : EventDataDocument
{
    public GuildCreateEventData(byte[] data) : base(data) { }

    #region 프로퍼티

    #nullable disable
    /// <summary>
    /// [ unavailable? (boolean) ] <br/>
    /// 길드가 사용 불가능한 상태인지 여부 <br/>
    /// true if this guild is unavailable due to an outage
    /// </summary>
    [EventField("unavailable")]
    public bool isUnavailable;

    public Snowflake GuildId
        => data.GetProperty("id").GetSnowflake();

    #nullable enable
    #endregion
}

public class AvailableGuildCreateEventData : EventDataDocument
{
    // Static
    public static EventDataDocumentMetadata? metadataStatic;


    #region Instance

    // 재정의
    protected override EventDataDocumentMetadata GetMetadata()
    {
        if (metadataStatic == null)
            metadataStatic = new EventDataDocumentMetadata(typeof(AvailableGuildCreateEventData));

        return metadataStatic;
    }
    

    #region 필드
    #nullable disable

    /// <summary>
    /// [ joined_at (ISO8601 timestamp) ] <br/>
    /// 봇이 서버에 가입한 시간 <br/>
    /// When this guild was joined at
    /// </summary>
    [EventField("joined_at")]
    public DeferredParseDateTimeProperty joinedAt;

    /// <summary>
    /// [ large (boolean) ] <br/>
    /// 길드가 "대규모"인지 여부 <br/>
    /// true if this is considered a large guild
    /// </summary>
    [EventField("large")]
    public bool isLarge;

    /// <summary>
    /// [ member_count (integer) ] <br/>
    /// 이 길드에 속한 총 멤버 수 <br/>
    /// Total number of members in this guild
    /// </summary>
    //public int MemberCount 
    //    => data.GetProperty("member_count").GetInt32();

    /// <summary>
    /// [ voice_states ] <br/>
    /// 음성 채널에 있는 멤버들의 상태 목록 <br/>
    /// States of members currently in voice channels
    /// </summary>
    //public JsonSourceArraySource<VoiceStateObject> VoiceStates
    //    => data.FindJsonSourceArraySource<VoiceStateObject>("voice_states");

    /// <summary>
    /// [ members ] <br/>
    /// 이 길드에 속한 사용자 목록 <br/>
    /// Users in the guild
    /// </summary>
    //public JsonSourceArraySource<GuildMemberObject> Members 
    //    => data.FindJsonSourceArraySource<GuildMemberObject>("members");

    /// <summary>
    /// [ channels ] <br/>
    /// 이 길드의 채널 목록 <br/>
    /// Channels in the guild
    /// </summary>
    //public JsonSourceArraySource<ChannelObject> Channels
    //    => data.FindJsonSourceArraySource<ChannelObject>("channels");

    /// <summary>
    /// [ threads ] <br/>
    /// 사용자가 볼 수 있는 모든 활성 스레드 <br/>
    /// All active threads in the guild the current user can view
    /// </summary>
    //public JsonSourceArraySource<ChannelObject> Threads 
    //    => data.FindJsonSourceArraySource<ChannelObject>("threads");
    
    /// <summary>
    /// [ presences ] <br/>
    /// 현재 상태 정보 (대규모 길드인 경우 오프라인은 포함되지 않음) <br/>
    /// Presences of the members in the guild
    /// </summary>
    //public JsonSourceArraySource<PresenceUpdateObject> Presences 
    //    => jsonElement.FindJsonSourceArraySource<PresenceUpdateObject>("presences");

    /// <summary>
    /// [ stage_instances ] <br/>
    /// 스테이지 인스턴스 정보 <br/>
    /// Stage instances in the guild
    /// </summary>
    //public JsonSourceArraySource<StageInstanceObject> StageInstances 
    //    => jsonElement.FindJsonSourceArraySource<StageInstanceObject>("stage_instances");

    /// <summary>
    /// [ guild_scheduled_events ] <br/>
    /// 예정된 길드 이벤트 목록 <br/>
    /// Scheduled events in the guild
    /// </summary>
    //public JsonSourceArraySource<GuildScheduledEventObject> ScheduledEvents 
    //    => jsonElement.FindJsonSourceArraySource<GuildScheduledEventObject>("guild_scheduled_events");

    /// <summary>
    /// [ soundboard_sounds ] <br/>
    /// 사운드보드 사운드 목록 <br/>
    /// Soundboard sounds in the guild
    /// </summary>
    //public JsonSourceArraySource<SoundboardSoundObject> SoundboardSounds 
    //    => jsonElement.FindJsonSourceArraySource<SoundboardSoundObject>("soundboard_sounds");

    #nullable enable
    #endregion


    // 생성자
    public AvailableGuildCreateEventData(
        ReadOnlyMemory<byte> data) : base(data) { }

    #endregion
}

public class UnavailableGuildCreateEventData : EventDataDocument
{
    // Static
    public static EventDataDocumentMetadata metadataStatic = new(typeof(UnavailableGuildCreateEventData));


    #region Instance
    
    // 재정의
    protected override EventDataDocumentMetadata GetMetadata()
    {
        if (metadataStatic == null)
            metadataStatic = new EventDataDocumentMetadata(typeof(AvailableGuildCreateEventData));

        return metadataStatic;
    }
    
    // 생성자
    public UnavailableGuildCreateEventData(
        ReadOnlyMemory<byte> data) : base(data) { }
        
    #endregion
}