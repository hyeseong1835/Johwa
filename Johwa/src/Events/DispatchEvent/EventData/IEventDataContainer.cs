namespace Johwa.Event.Data;

public interface IEventDataContainer : IDisposable
{
    public static List<EventPropertyDescriptorAttribute> LoadDescriptors<T>() 
        where T : IEventDataContainer
        => LoadDescriptors(typeof(T));
    public static List<EventPropertyDescriptorAttribute> LoadDescriptors(Type type)
    {
        List<EventPropertyDescriptorAttribute> result = new();

        // 필드 정보 가져오기
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyDescriptorAttribute를 가진 필드만 필터링
            EventPropertyDescriptorAttribute? descriptor = field.GetCustomAttribute<EventPropertyDescriptorAttribute>();
            if (descriptor == null)
                continue;

            result.Add(descriptor);
        }
        return result;
    }
    
    public abstract IEnumerable<EventPropertyData> GetPropertyDataEnumerable();
    abstract void IDisposable.Dispose();
}