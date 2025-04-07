namespace Johwa.Resources.Application;

/// <summary>
/// 앱이 설치될 수 있는 위치 <br/>
/// Where an app can be installed, also called its supported installation contexts.
/// </summary>
public enum ApplicationIntegrationType
{
    /// <summary>
    /// 서버에 설치 가능한 앱 <br/>
    /// App is installable to servers
    /// </summary>
    GuildInstall = 0,

    /// <summary>
    /// 사용자에게 설치 가능한 앱 <br/>
    /// App is installable to users
    /// </summary>
    UserInstall = 1
}
