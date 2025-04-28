namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
public class EventFieldAttribute : Attribute
{
    // 필드 & 프로퍼티
    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    // 생성자
    public EventFieldAttribute(string name, bool isOptional = false, bool isNullable = false)
    {
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }
    public EventFieldInfo CreateDescriptor()
        => new EventFieldInfo(name, isOptional, isNullable);
}