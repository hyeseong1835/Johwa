using System.Reflection;

namespace Johwa.Event.Data;

public class EventPropertyDescriptor : EventDataDescriptor
{
    #region Instance
    
    public override string Name => name;

    public readonly PropertyInfo propertyInfo;

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventPropertyDescriptor(PropertyInfo propertyInfo, string name, bool isOptional, bool isNullable)
    {
        this.propertyInfo = propertyInfo;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    #endregion
}