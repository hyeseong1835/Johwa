using System.Text.Json;
using Johwa.Common.Json.Reader;
using Johwa.Common.JsonSource;
using Johwa.Resources.Channel;
using Johwa.Resources.Guild;
using Johwa.Resources.Voice;

namespace Johwa.Event.DispatchEvents;

public ref struct GuildCreateEventData
{
    public GuildCreateEventData(ReadOnlySpan<byte> data)
    {
        Utf8JsonReader reader = new Utf8JsonReader(data);
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string? propName = reader.GetString();
                if (propName == null) continue;

                reader.Read(); 

                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        data = new JsonSourceObject(reader.ReadSpan());
                        break;
                    case JsonTokenType.StartArray:
                        data = new JsonSourceArray(reader.ReadSpan());
                        break;
                    case JsonTokenType.String:
                    {
                        switch (propName)
                        {
                            case "joined_at":
                                joinedAt = new DateTimeReader(reader.ValueSpan);
                                break;
                            default:
                                throw new NotSupportedException($"Unsupported property name: {propName}");
                        }
                        break;
                    }
                    case JsonTokenType.Number:
                    {
                        break;
                    }
                    case JsonTokenType.True:
                    {
                        switch (propName)
                        {
                            case "large":
                                isLarge = true;
                                break;
                            case "unavailable":
                                isUnavailable = true;
                                break;
                            default:
                                throw new NotSupportedException($"Unsupported property name: {propName}");
                        }
                        break;
                    }
                    case JsonTokenType.False:
                    {
                        switch (propName)
                        {
                            case "large":
                                isLarge = false;
                                break;
                            case "unavailable":
                                isUnavailable = false;
                                break;
                            default:
                                throw new NotSupportedException($"Unsupported property name: {propName}");
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported token type: {reader.TokenType}");
                }
                if (propName == "joined_at")
                {
                    joinedAt = new DateTimeReader(reader.ReadSpan());
                }
                else if (propName == "large")
                {
                    data.SetProperty("large", reader.ReadBoolean());
                }
                else if (propName == "unavailable")
                {
                    data.SetProperty("unavailable", reader.ReadBoolean());
                }
            }
        }
    }

    public GuildObject? Guild{ get {
        if (IsUnavailable) {
            return null;
        }
        return new GuildObject(data);
    } }

    /// <summary>
    /// [ joined_at ] <br/>
    /// 봇이 서버에 가입한 시간 <br/>
    /// When this guild was joined at
    /// </summary>
    public DateTimeReader joinedAt;

    /// <summary>
    /// [ large ] <br/>
    /// 길드가 "대규모"인지 여부 <br/>
    /// true if this is considered a large guild
    /// </summary>
    public bool isLarge;

    /// <summary>
    /// [ unavailable? ] <br/>
    /// 길드가 사용 불가능한 상태인지 여부 <br/>
    /// true if this guild is unavailable due to an outage
    /// </summary>
    public bool isUnavailable;

    /// <summary>
    /// [ member_count ] <br/>
    /// 이 길드에 속한 총 멤버 수 <br/>
    /// Total number of members in this guild
    /// </summary>
    public int MemberCount 
        => data.GetProperty("member_count").GetInt32();

    /// <summary>
    /// [ voice_states ] <br/>
    /// 음성 채널에 있는 멤버들의 상태 목록 <br/>
    /// States of members currently in voice channels
    /// </summary>
    public JsonSourceArraySource<VoiceStateObject> VoiceStates
        => data.FindJsonSourceArraySource<VoiceStateObject>("voice_states");

    /// <summary>
    /// [ members ] <br/>
    /// 이 길드에 속한 사용자 목록 <br/>
    /// Users in the guild
    /// </summary>
    public JsonSourceArraySource<GuildMemberObject> Members 
        => data.FindJsonSourceArraySource<GuildMemberObject>("members");

    /// <summary>
    /// [ channels ] <br/>
    /// 이 길드의 채널 목록 <br/>
    /// Channels in the guild
    /// </summary>
    public JsonSourceArraySource<ChannelObject> Channels
        => data.FindJsonSourceArraySource<ChannelObject>("channels");

    /// <summary>
    /// [ threads ] <br/>
    /// 사용자가 볼 수 있는 모든 활성 스레드 <br/>
    /// All active threads in the guild the current user can view
    /// </summary>
    public JsonSourceArraySource<ChannelObject> Threads 
        => data.FindJsonSourceArraySource<ChannelObject>("threads");
    
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
}