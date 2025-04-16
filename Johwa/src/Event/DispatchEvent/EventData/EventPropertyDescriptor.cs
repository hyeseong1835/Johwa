using System.Reflection;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class EventPropertyAttribute : Attribute
{
    // 필드 & 프로퍼티
    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    // 생성자
    public EventPropertyAttribute(string name, bool isOptional = false, bool isNullable = false)
    {
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }
    public EventPropertyDescriptor CreateDescriptor(IEventDataGroup group, FieldInfo fieldInfo, bool isFieldTypeEventProperty)
        => new EventPropertyDescriptor(group, fieldInfo, isFieldTypeEventProperty, name, isOptional, isNullable);
}
public class EventPropertyDescriptor
{
    #region Static

    public static bool IsNameMatch(EventPropertyDescriptor descriptor, ReadOnlyMemory<byte> nameMemory)
    {
        if (descriptor.name.Length != nameMemory.Length)
            return false;

        ReadOnlySpan<byte> nameSpan = nameMemory.Span;
        for (int i = 0; i < descriptor.name.Length; i++)
        {
            if (nameSpan[i] != descriptor.name[i])
                return false;
        }

        return true;
    }

    #endregion


    #region Instance

    public readonly IEventDataGroup group;
    public readonly FieldInfo fieldInfo;
    public readonly bool isFieldTypeEventProperty;

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventPropertyDescriptor(IEventDataGroup group, FieldInfo fieldInfo, bool isFieldTypeEventProperty, string name, bool isOptional, bool isNullable)
    {
        this.group = group;
        this.fieldInfo = fieldInfo;
        this.isFieldTypeEventProperty = isFieldTypeEventProperty;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    #endregion
}