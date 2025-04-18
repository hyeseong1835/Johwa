using Johwa.Common.Debug;
using System.Text.Json;

namespace Johwa.Event.Data;

[EventFieldReader(typeof(string))]
public class StringField : EventField
{
    public StringField(CreateData createData)
    {
        switch (createData.tokenType)
        {
            case JsonTokenType.String: {
                string value = createData.data.Span.ToString();
                createData.descriptor.fieldInfo.SetValue(createData.declaringGroup, value);
                break;
            }
            default: {
                JohwaLogger.Log($"JsonTokenType이 잘못된 형식입니다. : {createData.tokenType}",
                    severity: LogSeverity.Error, stackTrace: true);
                break;
            }
        }
    }
}