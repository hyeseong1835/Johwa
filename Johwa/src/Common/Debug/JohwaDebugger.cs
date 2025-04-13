using System.Runtime.CompilerServices;

namespace Johwa.Common.Debug;

public class JohwaDebugger
{
    #region Static

    public static JohwaDebugger? Instance { get; }

    public static void Log(
        LogSeverity severity,
        string message,
        string detail = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        new LogInfo
        {
            severity = severity,
            message = message,
            detail = detail,
            callerInfo = new CallerInfo(memberName, filePath, lineNumber),
            
        };
    }
    public static void Log(Exception exception)
    {
        
    }
    public static void Log(LogInfo info)
    {

    }


    #endregion
    
    #region Instance

    public Action<string> onLog;
    public Action<string> onWarning;
    public Action<string> onError;

    #endregion
}