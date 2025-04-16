using Johwa.Common.Debug;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Event.Data.PropertyReaders;

[EventPropertyReader(typeof(bool))]
public class BoolPropertyReader : EventPropertyReader
{
    public override bool TryReadProperty(EventPropertyCreateData createData, [NotNullWhen(true)] out EventProperty? property)
    {
        switch (createData.tokenType)
        {
            case JsonTokenType.True: {
                createData.descriptor.fieldInfo.SetValue(createData.descriptor.group, true);

                property = new EventValueProperty<bool>(createData);
                return true;
            }
            case JsonTokenType.False: {
                createData.descriptor.fieldInfo.SetValue(createData.descriptor.group, false);

                property = new EventValueProperty<bool>(createData);
                return true;
            }
            default: {
                JohwaLogger.Log($"JsonTokenType이 잘못된 형식입니다. : {createData.tokenType}",
                    severity: LogSeverity.Error, stackTrace: true);
                
                property = null;
                return false;
            }
        }
    }
}