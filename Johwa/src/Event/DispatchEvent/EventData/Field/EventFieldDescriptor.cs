using System.Reflection;

namespace Johwa.Event.Data;

public class EventFieldDescriptor
{
    #region Static

    public static bool IsNameMatch(EventFieldDescriptor descriptor, ReadOnlyMemory<byte> nameMemory)
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

    public readonly FieldInfo fieldInfo;
    public readonly bool isFieldTypeEventProperty;

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventFieldDescriptor(FieldInfo fieldInfo, bool isFieldTypeEventProperty, string name, bool isOptional, bool isNullable)
    {
        this.fieldInfo = fieldInfo;
        this.isFieldTypeEventProperty = isFieldTypeEventProperty;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    #endregion
}