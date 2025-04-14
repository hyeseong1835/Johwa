using System.Runtime.CompilerServices;

namespace Johwa.Common.Debug;

public class JohwaLogger
{
    #region Static

    public static JohwaLogger? Instance { get; }

    public static void Log(
        LogSeverity severity,
        string message,
        string detail = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LogInfo logInfo = new LogInfo
        {
            severity = severity,
            message = message,
            detail = detail,
            callerInfo = new CallerInfo(memberName, filePath, lineNumber),
        };
        Log(logInfo);
    }
    public static void Log(LogSeverity severity, string message, Exception exception,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LogInfo logInfo = new LogInfo
        {
            severity = severity,
            message = message,
            detail = $"{exception.StackTrace}",
            callerInfo = new CallerInfo(memberName, filePath, lineNumber),
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