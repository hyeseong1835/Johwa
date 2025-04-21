using System.Reflection;

namespace Johwa.Event.Data;

public class EventFieldDescriptor : EventDataDescriptor
{
    #region Instance

    public override string Name => name;

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