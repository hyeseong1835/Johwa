using Johwa.Common.Debug;
using System.Text.Json;

namespace Johwa.Event.Data;

[EventFieldReader(typeof(bool))]
public class BoolField : EventField
{
    public BoolField(CreateData createData)
    {
        switch (createData.tokenType)
        {
            case JsonTokenType.True: {
                createData.descriptor.fieldInfo.SetValue(createData.declaringObject, true);
                break;
            }
            case JsonTokenType.False: {
                createData.descriptor.fieldInfo.SetValue(createData.declaringObject, false);
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