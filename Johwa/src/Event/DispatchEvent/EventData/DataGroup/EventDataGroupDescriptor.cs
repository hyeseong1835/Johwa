using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupInfo
{
    public int memoryOffset;

    public EventDataGroupInfo(FieldInfo fieldInfo)
    {
        this.fieldInfo = fieldInfo;
    }
}