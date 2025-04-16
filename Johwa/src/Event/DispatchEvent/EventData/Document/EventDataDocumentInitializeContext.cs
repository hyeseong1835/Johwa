using Johwa.Common.Debug;
using System.Linq.Expressions;
using System.Reflection;

namespace Johwa.Event.Data;

public struct EventDataDocumentInitializeData
{
    public EventDataDocument document;
    public ReadOnlyMemory<byte> data;

    public EventDataDocumentInitializeData(EventDataDocument document, ReadOnlyMemory<byte> data)
    {
        this.document = document;
        this.data = data;
    }
}
public abstract class EventDataDocumentInitializeContext
{
    #region Static

    public static Action<EventDataDocumentInitializeData> initializeDocumentAction = GetInitializeDocumentAction();
    static Action<EventDataDocumentInitializeData> GetInitializeDocumentAction()
    {
        MethodInfo? writeLineMethod = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

        ConstantExpression param = Expression.Constant("Hello from Expression!");
        MethodCallExpression call = Expression.Call(writeLineMethod!, param);

        Expression<Action<EventDataDocumentInitializeData>> lambda = Expression.Lambda<Action<EventDataDocumentInitializeData>>(call);

        action();
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

        return lambda.Compile();
    }

    #endregion

    public abstract void Init(IEventDataContainer container, IEventDataGroup group, ReadOnlyMemory<byte> data);
}