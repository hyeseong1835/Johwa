using Johwa.Common.Debug;
using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data.PropertyReaders;

[EventPropertyReader(typeof(string))]
public class StringPropertyReader : EventPropertyReader
{
    public override bool TryReadProperty(FieldInfo fieldInfo, IEventDataGroup group, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        switch (tokenType)
        {
            case JsonTokenType.String:
                string value = data.Span.ToString();
                fieldInfo.SetValue(group, value);
                
                return true;

            default:
                JohwaLogger.Log($"JsonTokenType이 잘못된 형식입니다. : {tokenType}",
                    severity: LogSeverity.Error, stackTrace: true);

                return false;
        }
    }
}