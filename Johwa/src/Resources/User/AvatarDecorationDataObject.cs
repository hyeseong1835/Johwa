using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.User;

/// <summary>
/// 사용자의 아바타 장식에 대한 데이터입니다. <br/>
/// The data for the user's avatar decoration.
/// </summary>
public struct AvatarDecorationDataObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public AvatarDecorationDataObject(JsonElement avatarDecorationDataProperty)
    {
        this.Property = avatarDecorationDataProperty;
    }

    /// <summary>
    /// [ asset ] <br/>
    /// 아바타 장식의 해시 값 <br/>
    /// the avatar decoration hash
    /// </summary>
    public string Asset => Property.GetProperty("asset").GetString()!;

    /// <summary>
    /// [ sku_id ] <br/>
    /// 아바타 장식의 SKU ID <br/>
    /// id of the avatar decoration's SKU
    /// </summary>
    public ulong SkuId => Property.GetProperty("sku_id").GetUInt64();
}
