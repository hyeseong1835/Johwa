using System.Text.Json;
using Johwa.Common.JsonSource;

namespace Johwa.Resources.Channel;

/// <summary>
/// 역할 또는 사용자에 대한 권한 덮어쓰기 구조체 <br/>
/// Used to represent permission overwrites for a role or a user.
/// </summary>
public struct Overwrite : IJsonSource
{
    public JsonElement Property { get; set; }

    public Overwrite(JsonElement overwriteProperty)
    {
        this.Property = overwriteProperty;
    }

    /// <summary>
    /// [ id ] <br/>
    /// 역할 또는 사용자 ID <br/>
    /// role or user id
    /// </summary>
    public ulong Id => Property.GetProperty("id").GetUInt64();

    /// <summary>
    /// [ type ] <br/>
    /// 대상 타입 <br/>
    /// target type
    /// </summary>
    public OverwriteTargetType Type => (OverwriteTargetType)Property.GetProperty("type").GetInt32();

    /// <summary>
    /// [ allow ] <br/>
    /// 허용된 권한 비트 세트 <br/>
    /// permission bit set
    /// </summary>
    public string Allow => Property.GetProperty("allow").GetString()!;

    /// <summary>
    /// [ deny ] <br/>
    /// 거부된 권한 비트 세트 <br/>
    /// permission bit set
    /// </summary>
    public string Deny => Property.GetProperty("deny").GetString()!;
}
