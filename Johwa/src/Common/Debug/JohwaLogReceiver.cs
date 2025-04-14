namespace Johwa.Common.Debug;

public class JohwaLogReceiver
{
    public virtual void ReceiveLog(LogInfo info)
    {
        Console.WriteLine($"[{info.severity}] {info.message} \n" +
                          $"{info.detail} \n" +
                          $"{info.stackTrace}");
    }
}