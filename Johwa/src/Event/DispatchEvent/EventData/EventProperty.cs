using System.Reflection;
using System.Text.Json;
using Johwa.Common.Extension.System;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class EventPropertyDescriptorAttribute : Attribute
{
    #region Static
    
    public static bool IsNameMatch(EventPropertyDescriptorAttribute descriptor, ReadOnlyMemory<byte> nameMemory)
    {
        if (descriptor.name.Length != nameMemory.Length)
            return false;

        ReadOnlySpan<byte> nameSpan = nameMemory.Span;
        ReadOnlySpan<byte> descriptorNameSpan = descriptor.name.Span;
        for (int i = 0; i < descriptor.name.Length; i++)
        {
            if (nameSpan[i] != descriptorNameSpan[i])
                return false;
        }

        return true;
    }

    #endregion

    // 필드 & 프로퍼티
    public abstract EventPropertyMetadata PropertyMetaData { get; }
    public abstract Type PropertyMetaDataType { get; }

    public readonly FieldInfo fieldInfo;
    public readonly Memory<byte> name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    // 생성자
    public EventPropertyDescriptorAttribute(FieldInfo fieldInfo, string name, bool isOptional = false, bool isNullable = false)
    {
        this.fieldInfo = fieldInfo;
        name.CopyToByteMemory(this.name);
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    public abstract EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType);
}
public abstract class EventPropertyMetadata
{
    #region Static

    static Dictionary<Type, EventPropertyMetadata> instanceDictionary = new();
    public static EventPropertyMetadata GetInstance<T>() 
        where T : EventPropertyMetadata, new()
    {
        Type type = typeof(T);
        if (instanceDictionary.TryGetValue(type, out EventPropertyMetadata? result) == false)
        {
            result = new T();
            instanceDictionary[type] = result;
        }
        return result;
    }
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