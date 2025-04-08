using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.Channel;

/// <summary>
/// 팔로우된 채널을 나타내는 객체입니다. <br/>
/// Represents a followed channel returned from following a news channel to a target channel.
/// </summary>
public struct FollowedChannelObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public FollowedChannelObject(JsonElement followedChannelProperty)
    {
        this.Property = followedChannelProperty;
    }

    /// <summary>
    /// [ channel_id ] <br/>
    /// 소스 채널 ID <br/>
    /// source channel id
    /// </summary>
    public ulong ChannelId
    {
        get { return Property.GetProperty("channel_id").GetUInt64(); }
    }

    /// <summary>
    /// [ webhook_id ] <br/>
    /// 생성된 대상 웹후크 ID <br/>
    /// created target webhook id
    /// </summary>
    public ulong WebhookId
    {
        get { return Property.GetProperty("webhook_id").GetUInt64(); }
    }
}
