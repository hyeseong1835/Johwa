namespace Johwa.Common.Debug;

public abstract class JohwaLogReceiver
{
    public virtual void ReceiveLog(LogInfo info)
    {
        Console.WriteLine($"[{info.severity}] {info.message}");
    }
}