using System.Text.Json;
using Johwa.Common.Json;
using Johwa.Utility.StringResourceArrayUtility;

namespace Johwa.Resources.Application;

/// <summary>
/// 설치 시 요청할 권한 및 스코프를 정의하는 객체 <br/>
/// Install params for each installation context's default in-app authorization link
/// </summary>
public struct InstallParamsObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public InstallParamsObject(JsonElement installParamsProperty)
    {
        this.Property = installParamsProperty;
    }

    /// <summary>
    /// 애플리케이션을 서버에 추가할 때 사용할 OAuth2 스코프 목록 <br/>
    /// Scopes to add the application to the server with
    /// </summary>
    public StringArraySource Scopes 
        => Property.GetProperty("scopes").GetStringResourceArray();

    /// <summary>
    /// 봇 역할에 요청할 권한 <br/>
    /// Permissions to request for the bot role
    /// </summary>
    public string Permissions 
        => Property.GetProperty("permissions").GetString()!;
}
