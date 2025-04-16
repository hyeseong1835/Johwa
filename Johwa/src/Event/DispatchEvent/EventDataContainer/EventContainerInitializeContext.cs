using Johwa.Common.Debug;
using System.Reflection;

namespace Johwa.Event.Data;

public struct EventDataGroupInitializeData
{
    public IEventDataContainer container;
    public IEventDataGroup group;
    public ReadOnlyMemory<byte> data;

    public EventDataGroupInitializeData(IEventDataGroup group, ReadOnlyMemory<byte> data)
    {
        this.group = group;
        this.data = data;
    }
}
public abstract class EventDataGroupInitializeContext
{
    #region Static

    public static Action<IEventDataGroup, ReadOnlyMemory<byte>> initializeGroupAction;
    static Action<IEventDataGroup, ReadOnlyMemory<byte>>
    public static EventDataGroupInitializeContext[] instatnceArray = GetInstanceList().ToArray();
    static List<EventDataGroupInitializeContext> GetInstanceList()
    {
        List<EventDataGroupInitializeContext> result = new ();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            Type[] types = assembly.GetTypes();

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type type = types[typeIndex];

                // 클래스가 아니면 제외
                if (type.IsClass == false)
                    continue;

                // 추상 클래스 제외
                if (type.IsAbstract)
                    continue;

                // EventGroupInitializeContextMetadata 상속 클래스가 아니면 제외
                if (type.IsSubclassOf(typeof(EventDataGroupInitializeContext)) == false)
                    continue;

                EventDataGroupInitializeContext? metadata = Activator.CreateInstance(type) as EventDataGroupInitializeContext;
                if (metadata == null) {
                    JohwaLogger.Log($"{type} (EventGroupInitializeContextMetadata) 생성을 실패하였습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    continue;
                }

                result.Add(metadata);
            }
        }
        return result;
    }

    #endregion

    public abstract void Init(IEventDataContainer container, IEventDataGroup group, ReadOnlyMemory<byte> data);
}