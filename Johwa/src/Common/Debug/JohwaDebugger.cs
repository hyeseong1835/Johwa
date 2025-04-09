namespace Johwa.Common.Debug;

public class JohwaDebugger
{
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void LogError(string message)
    {
        Console.WriteLine($"Error: {message}");
    }

    public static void LogWarning(string message)
    {
        Console.WriteLine($"Warning: {message}");
    }

    public static void LogInfo(string message)
    {
        Console.WriteLine($"Info: {message}");
    }
}