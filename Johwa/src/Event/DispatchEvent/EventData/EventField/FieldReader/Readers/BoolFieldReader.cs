using Johwa.Common.Debug;
using System.Text.Json;

namespace Johwa.Event.Data;

public class BoolField : EventFieldReader<bool>
{
    unsafe protected override bool Read(ReadData data)
    {
        switch (data.jsonTokenType)
        {
            case JsonTokenType.True: {
                return true;
            }
            case JsonTokenType.False: {
                return false;
            }
            default: {
                JohwaLogger.Log($"JsonTokenType이 잘못된 형식입니다. : {data.jsonTokenType}",
                    severity: LogSeverity.Error, stackTrace: true);
                return false;
            }
        }
    }
}