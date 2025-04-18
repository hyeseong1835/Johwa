using System.Reflection;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Property, Inherited = true)]
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
    public EventPropertyDescriptor CreateDescriptor(PropertyInfo propertyInfo)
        => new EventPropertyDescriptor(propertyInfo, name, isOptional, isNullable);
}