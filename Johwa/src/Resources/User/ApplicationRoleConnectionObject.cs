using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.User;

/// <summary>
/// 애플리케이션이 사용자에게 연결한 역할 연결 객체입니다. <br/>
/// The role connection object that an application has attached to a user.
/// </summary>
public struct ApplicationRoleConnectionObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ApplicationRoleConnectionObject(JsonElement applicationRoleConnectionProperty)
    {
        this.Property = applicationRoleConnectionProperty;
    }

    /// <summary>
    /// [ platform_name? ] <br/>
    /// 봇이 연결한 플랫폼의 커스텀 이름 (최대 50자) <br/>
    /// the vanity name of the platform a bot has connected (max 50 characters)
    /// </summary>
    public string? PlatformName
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("platform_name", out prop) == false)
                return null;

            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            return prop.GetString();
        }
    }

    /// <summary>
    /// [ platform_username? ] <br/>
    /// 봇이 연결한 플랫폼의 사용자 이름 (최대 100자) <br/>
    /// the username on the platform a bot has connected (max 100 characters)
    /// </summary>
    public string? PlatformUsername
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("platform_username", out prop) == false)
                return null;

            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            return prop.GetString();
        }
    }

    /// <summary>
    /// [ metadata ] <br/>
    /// 봇이 연결한 플랫폼의 사용자에 대해, 애플리케이션 역할 연결 메타데이터 키와 문자열 값의 매핑 <br/>
    /// object mapping application role connection metadata keys to their string-ified value (max 100 characters) for the user on the platform a bot has connected
    /// </summary>
    public JsonElement Metadata => Property.GetProperty("metadata");
}