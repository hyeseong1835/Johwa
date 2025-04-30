using Johwa.Event.Data;

namespace Johwa.Event;

public delegate void Event<TData>(in TData eventData);

public class EventHandleContext<TData>
    where TData : unmanaged, IEventDataDocument
{
    public DiscordGatewayClient gatewayClient;
    public int enabledHandlerCount;
    public List<EventHandler<TData>> handlers;

    public EventHandleContext(DiscordGatewayClient gatewayClient, List<EventHandler<TData>> handlers, int enabledHandlerCount)
    {
        this.gatewayClient = gatewayClient;
        this.handlers = handlers;
        this.enabledHandlerCount = enabledHandlerCount;
    }
}
public class EventHandler<TData> : IDisposable
    where TData : unmanaged, IEventDataDocument
{
    #region Static

    static List<EventHandleContext<TData>>? contextList;
    public static EventHandleContext<TData> AddHandler(DiscordGatewayClient gatewayClient, EventHandler<TData> handler)
    {
        if (contextList == null) 
        {
            EventHandleContext<TData> info = new EventHandleContext<TData>(gatewayClient, [ handler ], 1);
            contextList = [ info ];
            return info;
        }
        else
        {
            EventHandleContext<TData> info;
            for (int i = 0; i < contextList.Count; i++)
            {
                info = contextList[i];
                if (info.gatewayClient == gatewayClient) {
                    info.handlers.Add(handler);
                    return info;
                }
            }
            info = new EventHandleContext<TData>(gatewayClient, [ handler ], 1);
            contextList.Add(info);
            return info;
        }
    }
    public static EventHandleContext<TData>? GetContext(DiscordGatewayClient gatewayClient)
    {
        if (contextList == null) return null;

        for (int i = 0; i < contextList.Count; i++)
        {
            EventHandleContext<TData> info = contextList[i];
            if (info.gatewayClient == gatewayClient) return info;
        }
        return null;
    }
    public static void OnHandled(DiscordGatewayClient gatewayClient, in TData eventData)
    {
        EventHandleContext<TData>? context = GetContext(gatewayClient);
        if (context == null) return;
        if (context.enabledHandlerCount == 0) return;

        for (int i = 0; i < context.handlers.Count; i++)
        {
            EventHandler<TData> handler = context.handlers[i];
            if (handler.IsEnabled) 
            {
                handler.onHandled.Invoke(in eventData);
            }
        }

        Console.WriteLine($"Event handled: {eventData.GetType().Name}");
    }

    #endregion

    #region Instance

    // 재정의
    public void Dispose()
    {
        if (contextList == null) return;

        IsEnabled = false;
        context.handlers.Remove(this);

        GC.SuppressFinalize(true);
    }

    // 필드
    public EventHandleContext<TData> context;
    public Event<TData> onHandled;
    public bool IsEnabled { 
        get => IsEnabled;
        set => SetEnable(value);
    }
    bool isEnabled = false;

    // 생성자
    public EventHandler(DiscordGatewayClient gatewayClient, Event<TData> onHandled, bool isEnabled = true)
    {
        this.onHandled = onHandled;
        this.isEnabled = isEnabled;
        this.context = AddHandler(gatewayClient, this);
    }
    
    // 종료자
    ~EventHandler()
    {
        if (contextList == null) return;
        
        Dispose();
    }

    public void SetEnable(bool enable)
    {
        if (isEnabled == enable) return;

        isEnabled = enable;
        
        if (enable) 
        {
            context.enabledHandlerCount++;
        } 
        else 
        {
            context.enabledHandlerCount--;
        }
    }
    
    #endregion
}