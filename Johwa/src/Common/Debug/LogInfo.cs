namespace Johwa.Common.Debug;

public struct LogInfo
{
    public LogSeverity severity;
    public string message;
    public string detail;
    public CallerInfo callerInfo;
}