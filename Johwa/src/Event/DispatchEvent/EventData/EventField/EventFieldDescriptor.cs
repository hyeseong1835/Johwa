using System.Reflection;

namespace Johwa.Event.Data;

public class EventFieldInfo : EventDataInfo
{
    #region Instance

    public readonly FieldInfo fieldInfo;

    public EventFieldInfo(FieldInfo fieldInfo, string name, bool isOptional, bool isNullable)
        : base(name, fieldInfo.DeclaringType!, isOptional, isNullable)
    {
        this.fieldInfo = fieldInfo;
    }

    public override EventData? ReadData(EventField.EventFieldCreateData createData)
    {
        return EventField.CreateInstance(createData);
    }

    #endregion
}