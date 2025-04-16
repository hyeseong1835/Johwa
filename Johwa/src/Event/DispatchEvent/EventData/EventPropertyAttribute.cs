using System.Reflection;

namespace Johwa.Event.Data;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class EventPropertyAttribute : Attribute
{
    #region Static

    public static bool IsNameMatch(EventPropertyAttribute descriptor, ReadOnlyMemory<byte> nameMemory)
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

    // 필드 & 프로퍼티
    public FieldInfo? fieldInfo;
    public bool isFieldTypeEventProperty;
    public Type? metadataType;
    public EventPropertyMetadata? dataMetadata;

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
    public void Init(FieldInfo fieldInfo, Type metadataType)
    {
        this.fieldInfo = fieldInfo;
        this.isFieldTypeEventProperty = fieldInfo.FieldType.IsSubclassOf(typeof(EventProperty));
        this.metadataType = metadataType;
    }
}