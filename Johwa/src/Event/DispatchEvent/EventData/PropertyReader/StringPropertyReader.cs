using Johwa.Common.Debug;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Johwa.Event.Data.PropertyReaders;

[EventPropertyReader(typeof(string))]
public class StringPropertyReader : EventPropertyReader
{
    public override bool TryReadProperty(EventPropertyCreateData createData, [NotNullWhen(true)] out EventProperty? property)
    {
        switch (createData.tokenType)
        {
            case JsonTokenType.String: {
                string value = createData.data.Span.ToString();
                createData.descriptor.fieldInfo.SetValue(createData.descriptor.group, value);

                property = new EventValueProperty<string>(createData);
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