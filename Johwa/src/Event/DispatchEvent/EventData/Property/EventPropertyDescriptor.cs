using System.Reflection;

namespace Johwa.Event.Data;

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

    public readonly PropertyInfo propertyInfo;
    public readonly bool isFieldTypeEventProperty;

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventPropertyDescriptor(PropertyInfo propertyInfo, bool isFieldTypeEventProperty, string name, bool isOptional, bool isNullable)
    {
        this.propertyInfo = propertyInfo;
        this.isFieldTypeEventProperty = isFieldTypeEventProperty;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    #endregion
}