using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data;

public class EventFieldInfo : EventDataInfo
{
    #region Instance

    public FieldInfo fieldInfo;

    public EventFieldInfo(FieldInfo fieldInfo, string name, bool isOptional, bool isNullable)
        : base(name, isOptional, isNullable)
    {
        this.fieldInfo = fieldInfo;
    }
    
    unsafe public override void ReadData<TDocument>(TDocument* documentPtr, ReadOnlyMemory<byte> jsonData, JsonTokenType jsonTokenType)
    {
        
    }

    #endregion
}