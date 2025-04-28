using System.Text.Json;

namespace Johwa.Event.Data;

public class EventFieldInfo : EventDataInfo
{
    #region Instance

    public EventFieldInfo(string name, bool isOptional, bool isNullable)
        : base(name, isOptional, isNullable) { }
    
    unsafe public override void ReadData<TDocument>(TDocument* documentPtr, ReadOnlyMemory<byte> jsonData, JsonTokenType jsonTokenType)
    {
        
    }

    #endregion
}