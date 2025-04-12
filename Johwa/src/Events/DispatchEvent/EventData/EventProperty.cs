using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class EventPropertyDescriptorAttribute : Attribute
{
    public abstract EventPropertyMetadata PropertyMetaData { get; }
    public abstract Type PropertyMetaDataType { get; }

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventPropertyDescriptorAttribute(string name, bool isOptional = false, bool isNullable = false)
    {
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    public abstract EventPropertyMetadata CreateMetadata(FieldInfo fieldInfo);
}
public abstract class EventPropertyMetadata
{
    #region Static

    static Dictionary<Type, EventPropertyMetadata> instanceDictionary = new();
    public static EventPropertyMetadata GetMetadata(EventPropertyDescriptorAttribute attribute, Type propertyMetaDataType, FieldInfo field)
    {
        if (instanceDictionary.TryGetValue(propertyMetaDataType, out EventPropertyMetadata? result) == false)
        {
            result = (EventPropertyMetadata?)Activator.CreateInstance(propertyMetaDataType, attribute, field);
            if (result == null)
                throw new InvalidOperationException("오류");
        }
        return result;
    }

    public static List<EventPropertyMetadata> LoadMetadata(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<EventPropertyMetadata> result = new();

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyAttribute를 가진 필드만 필터링
            EventPropertyDescriptorAttribute? attribute = field.GetCustomAttribute<EventPropertyDescriptorAttribute>();
            if (attribute == null)
                continue;

            // 메타데이터 타입 확인
            Type propertyMetaDataType = attribute.PropertyMetaDataType;
            if (propertyMetaDataType.IsSubclassOf(typeof(EventPropertyMetadata)) == false)
                throw new InvalidOperationException("오류");

            // 메타데이터 생성
            EventPropertyMetadata metadata = GetMetadata(attribute, propertyMetaDataType, field);
            result.Add(metadata);
        }
        return result;
    }
    
    public static bool IsNameMatchMetaData(EventPropertyMetadata metadata, ValueTuple<byte[], int> nameData)
    {
        EventPropertyDescriptorAttribute propertyAttribute = metadata.;
        if (metadata.Attribute.name.Length != nameData.Item2)
            return false;
        
        for (int i = 0; i < metadata.Attribute.name.Length; i++)
        {
            if (nameData.Item1[i] != metadata.Attribute.name[i]) 
                return false;
        }
        
        return true;
    }

    #endregion


    #region Instance

    public readonly FieldInfo fieldInfo;

    public EventPropertyMetadata(FieldInfo fieldInfo)
    {
        this.fieldInfo = fieldInfo;
    }

    public abstract EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType);

    #endregion
}
public abstract class EventPropertyData : IDisposable
{
    #region Instance

    public abstract EventPropertyDescriptorAttribute Descriptor { get; }

    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        GC.SuppressFinalize(this);
    }

    public ReadOnlyMemory<byte> data;

    public EventPropertyData(ReadOnlyMemory<byte> data)
    {
        this.data = data;
    }

    #endregion
}