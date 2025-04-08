using System.Text.Json;
using Johwa.Common.JsonSource;
using Johwa.Resources.User;
using Johwa.Utility.Json;

namespace Johwa.Resources.Guild;

/// <summary>
/// 길드 멤버 정보를 나타냅니다. <br/>
/// the user this guild member represents
/// </summary>
public struct GuildMemberObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public GuildMemberObject(JsonElement guildMemberProperty)
    {
        Property = guildMemberProperty;
    }

    /// <summary>
    /// [ user? ] <br/>
    /// 해당 길드 멤버가 나타내는 유저 객체 <br/>
    /// the user this guild member represents
    /// </summary>
    //public UserObject? User 
    //    => Property.FindJsonSourceOrNull<UserObject>("user");

    /// <summary>
    /// [ nick? ] <br/>
    /// 길드 내 유저의 닉네임 <br/>
    /// this user's guild nickname
    /// </summary>
    public string? Nick 
        => Property.FindStringOrNull("nick");

    /// <summary>
    /// [ avatar? ] <br/>
    /// 멤버의 길드 아바타 해시 <br/>
    /// the member's guild avatar hash
    /// </summary>
    public string? Avatar 
        => Property.FindStringOrNull("avatar");

    /// <summary>
    /// [ banner? ] <br/>
    /// 멤버의 길드 배너 해시 <br/>
    /// the member's guild banner hash
    /// </summary>
    public string? Banner 
         => Property.FindStringOrNull("banner");

    /// <summary>
    /// [ roles ] <br/>
    /// 역할 ID 배열 <br/>
    /// array of role object ids
    /// </summary>
    public SnowflakeArraySource Roles
        => new(Property.GetProperty("roles"));

    /// <summary>
    /// [ joined_at ] <br/>
    /// 유저가 길드에 가입한 시간 <br/>
    /// when the user joined the guild
    /// </summary>
    public DateTime JoinedAt 
        => Property.FindDateTime("joined_at");

    /// <summary>
    /// [ premium_since? ] <br/>
    /// 사용자가 서버 부스트를 시작한 시간 <br/>
    /// when the user started boosting the guild
    /// </summary>
    public DateTime? PremiumSince 
        => Property.FindDateTimeOrNull("premium_since");

    /// <summary>
    /// [ deaf ] <br/>
    /// 유저가 음성 채널에서 청각 제한 상태인지 여부 <br/>
    /// whether the user is deafened in voice channels
    /// </summary>
    public bool Deaf 
        => Property.FindBoolean("deaf");

    /// <summary>
    /// [ mute ] <br/>
    /// 유저가 음성 채널에서 음소거 상태인지 여부 <br/>
    /// whether the user is muted in voice channels
    /// </summary>
    public bool Mute 
        => Property.FindBoolean("mute");

    /// <summary>
    /// [ flags ] <br/>
    /// 길드 멤버 플래그 비트셋 <br/>
    /// guild member flags represented as a bit set
    /// </summary>
    public GuildMemberFlag Flags => (GuildMemberFlag)Property.FindInt("flags");

    /// <summary>
    /// [ pending? ] <br/>
    /// 사용자가 멤버십 확인 요구사항을 아직 통과하지 않았는지 여부 <br/>
    /// whether the user has not yet passed the guild's Membership Screening requirements
    /// </summary>
    public bool? Pending 
         => Property.FindBooleanOrNull("pending");

    /// <summary>
    /// [ permissions? ] <br/>
    /// 채널 내 총 권한 (오버라이드 포함) <br/>
    /// total permissions of the member in the channel, including overwrites, returned when in the interaction object
    /// </summary>
    public string? Permissions 
        => Property.FindStringOrNull("permissions");

    /// <summary>
    /// [ communication_disabled_until? ] <br/>
    /// 해당 유저의 타임아웃 만료 시점 <br/>
    /// when the user's timeout will expire and the user will be able to communicate in the guild again
    /// </summary>
    public DateTime? CommunicationDisabledUntil
        => Property.FindDateTimeOrNull("communication_disabled_until");

    /// <summary>
    /// [ avatar_decoration_data? ] <br/>
    /// 유저의 길드 아바타 장식 데이터 <br/>
    /// data for the member's guild avatar decoration
    /// </summary>
    public AvatarDecorationDataObject? AvatarDecorationData 
        => Property.FindJsonSourceOrNull<AvatarDecorationDataObject>("avatar_decoration_data");

}
