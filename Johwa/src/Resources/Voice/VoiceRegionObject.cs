using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.Voice;

/// <summary>
/// 음성 지역 정보를 나타내는 객체입니다. <br/>
/// Unique identifier and properties of a Discord voice region.
/// </summary>
public struct VoiceRegionObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public VoiceRegionObject(JsonElement voiceRegionProperty)
    {
        this.Property = voiceRegionProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 해당 지역의 고유 ID <br/>
    /// unique ID for the region
    /// </summary>
    public string Id => Property.GetProperty("id").GetString()!;

    /// <summary>
    /// [ name ] <br/>
    /// 지역의 이름 <br/>
    /// name of the region
    /// </summary>
    public string Name => Property.GetProperty("name").GetString()!;

    /// <summary>
    /// [ optimal ] <br/>
    /// 현재 사용자에게 가장 가까운 서버인지 여부 <br/>
    /// true for a single server that is closest to the current user's client
    /// </summary>
    public bool Optimal => Property.GetProperty("optimal").GetBoolean();

    /// <summary>
    /// [ deprecated ] <br/>
    /// 더 이상 사용되지 않는 지역인지 여부 <br/>
    /// whether this is a deprecated voice region (avoid switching to these)
    /// </summary>
    public bool Deprecated => Property.GetProperty("deprecated").GetBoolean();

    /// <summary>
    /// [ custom ] <br/>
    /// 커스텀 이벤트 등에 사용되는 사용자 정의 지역인지 여부 <br/>
    /// whether this is a custom voice region (used for events/etc)
    /// </summary>
    public bool Custom => Property.GetProperty("custom").GetBoolean();
}
