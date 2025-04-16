using System.Reflection;
using Johwa.Common.Debug;

namespace Johwa.Event.Data;

public abstract class EventPropertyMetadata
{
    #region Static

    public static Dictionary<Type, EventPropertyMetadata> instanceDictionary = new();

    #endregion

    public Type propertyType;

    public EventPropertyMetadata(Type propertyType)
    {
        this.propertyType = propertyType;

        if (propertyType.IsAbstract) {
            JohwaLogger.Log($"{propertyType.Name} (EventPropertyData)는 추상 클래스일 수 없습니다. : {propertyType}",
                severity: LogSeverity.Warning, stackTrace: true);
        }
    }
}
public abstract class EventProperty : IDisposable
{
    public class Info
    {
        public EventPropertyAttribute descriptor;
        public ReadOnlyMemory<byte> data;

        public Info(EventPropertyAttribute descriptor, ReadOnlyMemory<byte> data)
        {
            this.descriptor = descriptor;
            this.data = data;
        }
    }
    #region Instance

    void IDisposable.Dispose()
    {
        if (info != null)
        {
            info.data = default;
            info = null;
        }
    }

    public Info? info;

    public virtual void Init(Info info)
    {
        this.info = info;
    }
    public abstract EventPropertyMetadata? CreateMetadata(FieldInfo fieldInfo);

    #endregion
}
public abstract class EventProperty<T> : EventProperty
{
    public T value;
    public override void Init(Info info)
    {
        base.Init(info);
        value = default!;
    }
}