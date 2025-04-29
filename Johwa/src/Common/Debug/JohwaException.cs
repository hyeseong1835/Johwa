namespace Johwa.Common.Debug;

public class JohwaException : Exception
{
    public string message;
    public string? detail;
    public LogSeverity severity;
    public bool stackTrace;

    public JohwaException(
        string message,
        string? detail = null,
        LogSeverity severity = LogSeverity.Info,
        bool stackTrace = false)
    {
        this.message = message;
        this.detail = detail;
        this.severity = severity;
        this.stackTrace = stackTrace;
    }
}