namespace Johwa.Common.Debug;

public class JohwaLogger
{
    #region Static

    public static JohwaLogger? Instance { get; }

    public static void Log(
        string message,
        string? detail = null,
        LogSeverity severity = LogSeverity.Info,
        bool stackTrace = false
        )
    {
        LogInfo logInfo = new LogInfo
        {
            severity = severity,
            message = message,
            detail = detail,
            stackTrace = stackTrace? Environment.StackTrace : null
        };
        Log(logInfo);
    }
    public static void Log(string message, LogSeverity severity, Exception exception, string? detail = null)
    {
        LogInfo logInfo = new LogInfo
        {
            severity = severity,
            message = message,
            detail = detail,
            stackTrace = exception.StackTrace
        };
        Log(logInfo);
    }
    public static void Log(LogInfo info)
    {
        JohwaLogger? logger = Instance;
        if (logger == null) 
            return;
        
        for (int i = 0; i < logger.receivers.Count; i++)
        {
            logger.receivers[i].ReceiveLog(info);
        }
    }


    #endregion
    
    #region Instance

    public List<JohwaLogReceiver> receivers = new List<JohwaLogReceiver>();

    #endregion
}