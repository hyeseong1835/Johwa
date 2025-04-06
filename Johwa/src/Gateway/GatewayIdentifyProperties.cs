
namespace Johwa.Gateway;

public struct GatewayIdentifyProperties
{
    // OS
    public readonly string os;
    public static string GetDefaultOs()
    {
        return Environment.OSVersion.Platform.ToString().ToLower();
    }
    
    // Browser
    public readonly string browser;
    public static string GetDefaultBrowser()
    {
        return "johwa";
    }
    
    // Device
    public readonly string device;
    public static string GetDefaultDevice()
    {
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return "desktop";
        if (OperatingSystem.IsBrowser())
            return "web";
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            return "mobile";

        return "johwa";
    }

    /// <summary>
    /// Discord Identify payload에 포함되는 클라이언트 환경 정보를 저장합니다. <br/>
    /// 각 필드는 지정되지 않은 경우, 현재 실행 환경을 기준으로 자동 설정됩니다. <br/>
    /// # OS: 운영체제 정보를 나타냅니다. <br/>
    /// <c>Environment.OSVersion.Platform.ToString().ToLower()</c> <br/>
    /// # Browser: Discord 클라이언트 또는 라이브러리 이름을 나타냅니다. <br/>
    /// <c>"johwa"</c> <br/>
    /// # Device: 장치 유형 또는 클라이언트 이름을 나타냅니다. <br/>
    /// • Windows/Linux/macOS → <c>"desktop"</c><br/>
    /// • WebAssembly → <c>"web"</c><br/>
    /// • Android/iOS → <c>"mobile"</c><br/>
    /// • 기타 → <c>"johwa"</c>
    /// </summary>
    public GatewayIdentifyProperties()
    {
        this.os = GetDefaultOs();
        this.browser = GetDefaultBrowser();
        this.device = GetDefaultDevice();
    }
    /// <summary>
    /// Discord Identify payload에 포함되는 클라이언트 환경 정보를 저장합니다. <br/>
    /// 각 필드는 지정되지 않은 경우, 현재 실행 환경을 기준으로 자동 설정됩니다.
    /// </summary>
    /// <param name="os">
    /// 운영체제 정보를 나타냅니다. <br/>
    /// <see langword="null"/>일 경우: <c>Environment.OSVersion.Platform.ToString().ToLower()</c>
    /// </param>
    /// <param name="browser">
    /// Discord 클라이언트 또는 라이브러리 이름을 나타냅니다. <br/>
    /// <see langword="null"/>일 경우: <c>"johwa"</c>
    /// </param>
    /// <param name="device">
    /// 장치 유형 또는 클라이언트 이름을 나타냅니다. <br/>
    /// <see langword="null"/>일 경우 다음 기준에 따라 자동 설정됩니다: <br/>
    /// • Windows/Linux/macOS → <c>"desktop"</c><br/>
    /// • WebAssembly → <c>"web"</c><br/>
    /// • Android/iOS → <c>"mobile"</c><br/>
    /// • 기타 → <c>"johwa"</c>
    /// </param>
    public GatewayIdentifyProperties(string? os = null, string? browser = null, string? device = null)
    {
        this.os = os?? GetDefaultOs();
        this.browser = browser?? GetDefaultBrowser();
        this.device = device?? GetDefaultDevice();
    }
}