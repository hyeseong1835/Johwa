using System.Text.Json;
using Johwa.Common.JsonSource;
using Johwa.Utility.Json;

namespace Johwa.Resources.Application;

/// <summary>
/// 설치 컨텍스트별 기본 설치 매개변수 정보 <br/>
/// Install params for each installation context's default in-app authorization link
/// </summary>
public struct ApplicationIntegrationTypeConfigurationObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ApplicationIntegrationTypeConfigurationObject(JsonElement integrationTypeConfigProperty)
    {
        this.Property = integrationTypeConfigProperty;
    }

    /// <summary>
    /// [ oauth2_install_params? (install params object) ] <br/>
    /// 해당 설치 컨텍스트의 설치 매개변수 <br/>
    /// Install params for each installation context's default in-app authorization link
    /// </summary>
    public InstallParamsObject? OAuth2InstallParams
        => Property.FindJsonSourceOrNull<InstallParamsObject>("oauth2_install_params");
}
