using System.Reflection;

namespace Johwa.Event.Data;

public class EventPropertyDescriptor : EventDataDescriptor
{
    #region Instance
    
    public readonly PropertyInfo propertyInfo;

    public EventPropertyDescriptor(PropertyInfo propertyInfo, string name, bool isOptional, bool isNullable)
        : base(name, propertyInfo.DeclaringType!, isOptional, isNullable)
    {
        this.propertyInfo = propertyInfo;
    }

    public override EventData? CreateData(EventData.EventDataCreateData createData)
    {
        return new EventProperty(createData);
    }

    #endregion
}