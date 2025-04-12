using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class EventPropertyAttribute : Attribute
{
    public abstract Type PropertyMetaDataType { get; }

    public abstract EventPropertyTypeMetadata CreateMetadata(FieldInfo fieldInfo);
}

public abstract class EventPropertyTypeMetadata
{
    #region Static

    public static Dictionary<Type, EventPropertyTypeMetadata> instanceDictionary = new();
    public static EventPropertyTypeMetadata GetMetadata(EventPropertyAttribute attribute, Type propertyMetaDataType, FieldInfo field)
    {
        if (instanceDictionary.TryGetValue(propertyMetaDataType, out EventPropertyTypeMetadata? result) == false)
        {
            result = (EventPropertyTypeMetadata?)Activator.CreateInstance(propertyMetaDataType, attribute, field);
            if (result == null)
                throw new InvalidOperationException("오류");
        }
        return result;
    }

    public static List<EventPropertyTypeMetadata> LoadMetadata(Type type)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<EventPropertyTypeMetadata> result = new();

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyAttribute를 가진 필드만 필터링
            EventPropertyAttribute? attribute = field.GetCustomAttribute<EventPropertyAttribute>();
            if (attribute == null)
                continue;

            // 메타데이터 타입 확인
            Type propertyMetaDataType = attribute.PropertyMetaDataType;
            if (propertyMetaDataType.IsSubclassOf(typeof(EventPropertyTypeMetadata)) == false)
                throw new InvalidOperationException("오류");

            // 메타데이터 생성
            EventPropertyTypeMetadata metadata = GetMetadata(attribute, propertyMetaDataType, field);
            result.Add(metadata);
        }
        return result;
    }
    
    public static bool IsNameMatchMetaData(EventPropertyTypeMetadata metadata, ValueTuple<byte[], int> nameData)
    {
        EventPropertyAttribute propertyAttribute = metadata.Attribute;
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

    public EventPropertyTypeMetadata(FieldInfo fieldInfo)
    {
        this.fieldInfo = fieldInfo;
    }

    public abstract EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType);

    #endregion
}
public class EventPropertyM
{
    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;
    
    public EventPropertyAttribute(string name, bool isOptional, bool isNullable)
    {
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }
}
public abstract class EventPropertyData : IDisposable
{
    #region Instance

    public abstract EventPropertyAttribute Attribute { get; }

    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        GC.SuppressFinalize(this);
    }

    public EventPropertyTypeMetadata propertyMetadata;
    public ReadOnlyMemory<byte> data;

    public EventPropertyData(EventPropertyTypeMetadata metadata, ReadOnlyMemory<byte> data)
    {
        this.propertyMetadata = metadata;
        this.data = data;
    }

    #endregion
}