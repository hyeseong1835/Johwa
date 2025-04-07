using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.User;

/// <summary>
/// 사용자가 연결한 외부 계정 정보를 나타냅니다. <br/>
/// The connection object that the user has attached.
/// </summary>
public struct ConnectionObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ConnectionObject(JsonElement connectionProperty)
    {
        this.Property = connectionProperty;
    }

    /// <summary>
    /// 연결 계정의 ID <br/>
    /// id of the connection account
    /// </summary>
    public string Id => Property.GetProperty("id").GetString()!;

    /// <summary>
    /// 연결 계정의 사용자 이름 <br/>
    /// the username of the connection account
    /// </summary>
    public string Name => Property.GetProperty("name").GetString()!;

    /// <summary>
    /// 연결된 서비스의 이름 <br/>
    /// the service of this connection
    /// </summary>
    public string Type => Property.GetProperty("type").GetString()!;

    /// <summary>
    /// [ revoked? ] <br/>
    /// 연결이 해제되었는지 여부 <br/>
    /// whether the connection is revoked
    /// </summary>
    public bool? Revoked { get {
        JsonElement prop;
        if (Property.TryGetProperty("revoked", out prop) == false)
            return null;
        if (prop.ValueKind == JsonValueKind.Null)
            return null;
        return prop.GetBoolean();
    } }

    /// <summary>
    /// [ integrations? ] <br/>
    /// 일부 서버 통합 객체 배열 <br/>
    /// an array of partial server integrations
    /// </summary>
    public ResourceArray<PartialServerIntegration>? Integrations { get {
        JsonElement prop;
        if (Property.TryGetProperty("integrations", out prop) == false)
            return null;
        if (prop.ValueKind == JsonValueKind.Null)
            return null;
        return new ResourceArray<PartialServerIntegration>(prop);
    } }

    /// <summary>
    /// 연결이 인증되었는지 여부 <br/>
    /// whether the connection is verified
    /// </summary>
    public bool Verified => Property.GetProperty("verified").GetBoolean();

    /// <summary>
    /// 친구 동기화가 활성화되어 있는지 여부 <br/>
    /// whether friend sync is enabled for this connection
    /// </summary>
    public bool FriendSync => Property.GetProperty("friend_sync").GetBoolean();

    /// <summary>
    /// 이 연결과 관련된 활동이 상태 업데이트에 표시되는지 여부 <br/>
    /// whether activities related to this connection will be shown in presence updates
    /// </summary>
    public bool ShowActivity => Property.GetProperty("show_activity").GetBoolean();

    /// <summary>
    /// 이 연결이 대응하는 제3자 OAuth2 토큰을 가지고 있는지 여부 <br/>
    /// whether this connection has a corresponding third party OAuth2 token
    /// </summary>
    public bool TwoWayLink => Property.GetProperty("two_way_link").GetBoolean();

    /// <summary>
    /// 연결의 표시 상태 <br/>
    /// visibility of this connection
    /// </summary>
    public int Visibility => Property.GetProperty("visibility").GetInt32();
}
