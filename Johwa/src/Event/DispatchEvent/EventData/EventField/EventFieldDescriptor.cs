using System.Reflection;

namespace Johwa.Event.Data;

public class EventFieldDescriptor : EventDataDescriptor
{
    #region Instance

    public readonly FieldInfo fieldInfo;

    public EventFieldDescriptor(FieldInfo fieldInfo, string name, bool isOptional, bool isNullable)
        : base(name, fieldInfo.DeclaringType!, isOptional, isNullable)
    {
        this.fieldInfo = fieldInfo;
    }

    public override EventData? CreateData(EventField.EventFieldCreateData createData)
    {
        return EventField.CreateInstance(createData);
    }

    #endregion
}