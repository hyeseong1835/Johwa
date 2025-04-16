using Johwa.Common.Debug;
using System.Reflection;
using System.Text.Json;

namespace Johwa.Event.Data.PropertyReaders;

[EventPropertyReader(typeof(bool))]
public class BoolPropertyReader : EventPropertyReader
{
    public override bool TryReadProperty(FieldInfo fieldInfo, IEventDataGroup group, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        switch (tokenType)
        {
            case JsonTokenType.True:
                fieldInfo.SetValue(group, true);
                return true;

            case JsonTokenType.False:
                fieldInfo.SetValue(group, false);
                return true;

            default:
                JohwaLogger.Log($"JsonTokenType이 잘못된 형식입니다. : {tokenType}",
                    severity: LogSeverity.Error, stackTrace: true);
                return false;
        }
    }
}