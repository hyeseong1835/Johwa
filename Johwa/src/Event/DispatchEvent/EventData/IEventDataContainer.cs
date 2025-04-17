using Johwa.Event.Data.PropertyReaders;
using Johwa.Common.Debug;
using Johwa.Common.Extension.System.Text.Json;
using Johwa.Common.Collection;
using System.Text;
using System.Text.Json;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Event.Data;

public class EventDataContainerMetadata
{
    public static Dictionary<Type, EventDataContainerMetadata> instanceDictionary = new Dictionary<Type, EventDataContainerMetadata>();
    public static EventDataContainerMetadata GetInstance(Type dataType)
    {
        if (instanceDictionary.TryGetValue(dataType, out EventDataContainerMetadata? result) == false)
        {
            result = new EventDataContainerMetadata(dataType);
            instanceDictionary[dataType] = result;
        }
        return result;
    }

    public readonly EventFieldDescriptor[] propertyDescriptorArray;

    public EventDataContainerMetadata(Type dataType)
    {
        this.propertyDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(dataType);
    }
}

public abstract class EventDataContainer : IEventDataGroup
{
    #region Instance

    public EventDataContainerMetadata metadata;
    public List<EventField> fieldSet = new();

    public ReadOnlyMemory<byte> data;

    public List<EventField> CreatePropertySet()
    {
        ReadOnlySpan<byte> dataSpan = data.Span;

        // 결과 초기화
        List<EventField> result = new(metadata.propertyDescriptorArray.Length);

        // Json 읽기
        Utf8JsonReader jsonReader = new(dataSpan);
        
        // 노드 버퍼 (스택)
        Span<ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>>.LinkedListNode> nodeBuffer 
            = stackalloc ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>>.LinkedListNode[metadata.propertyDescriptorArray.Length];

        // 프로퍼티 탐색을 위한 세트 생성
        ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>> propertyDescriptorSet 
            = new(new ReadOnlyMemory<EventFieldDescriptor>(metadata.propertyDescriptorArray), nodeBuffer);

        // 프로퍼티 이름 버퍼 대여
        byte[] propertyNameBuffer = ArrayPool<byte>.Shared.Rent(64);
        try
        {
            // 읽기
            while (jsonReader.Read())
            {
                // 프로퍼티 이름이 아닐 경우 무시
                if (jsonReader.TokenType != JsonTokenType.PropertyName) continue;

                // 프로퍼티 이름 복사
                ReadOnlySpan<byte> valueSpan = jsonReader.ValueSpan;
                valueSpan.CopyTo(propertyNameBuffer);

                // 프로퍼티 메타데이터 탐색
                EventFieldDescriptor? propertyDescriptor;
                ReadOnlyMemory<byte> propertyName = new(propertyNameBuffer, 0, valueSpan.Length);
                if (propertyDescriptorSet.TryExtractValue(propertyName, out propertyDescriptor, EventFieldDescriptor.IsNameMatch) == false) {
                    // 프로퍼티 Descriptor를 찾을 수 없을 경우 예외 발생
                    JohwaLogger.Log($"이벤트 프로퍼티 '{Encoding.ASCII.GetString(propertyName.Span)}'를 찾을 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    continue;
                }

                // 불가능한 오류 (컴파일러 안심)
                if (propertyDescriptor == null) {
                    JohwaLogger.Log($"이벤트 프로퍼티 '{Encoding.ASCII.GetString(propertyName.Span)}'를 찾을 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    continue;
                }

                // 값으로 이동
                jsonReader.Read();

                // 프로퍼티 데이터 타입
                JsonTokenType propertyDataTokenType = jsonReader.TokenType;
                
                // 프로퍼티 데이터 자르기
                ReadOnlyMemory<byte> propertyJsonData = jsonReader.ReadAndSliceToken(data);

                // 프로퍼티 생성 데이터
                EventFieldCreateData propertyCreateData = new(data, propertyDescriptor, propertyJsonData, propertyDataTokenType);

                // 프로퍼티 생성
                EventField? property = null;
                if (CreateProperty(propertyCreateData, out property) == false)
                    continue;

                result.Add(property);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventFieldDescriptor propertyDescriptor in propertyDescriptorSet.GetEnumerable())
            {
                // 파라미터로 넘겨줄 데이터 : 이름이 비었으면 전체 데이터 전달
                ReadOnlyMemory<byte> propertyDataMemory = (propertyDescriptor.name == null)? data : ReadOnlyMemory<byte>.Empty;

                // 프로퍼티 생성
                EventFieldCreateData propertyCreateData = new(this, propertyDescriptor, propertyDataMemory, JsonTokenType.None);

                // 프로퍼티 리더
                EventPropertyReader? propertyReader = EventPropertyReader.GetInstance(propertyCreateData.descriptor.fieldInfo.FieldType);
                if (propertyReader == null) {
                    JohwaLogger.Log($"프로퍼티 리더를 찾을 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);

                    continue;
                }

                // 프로퍼티 생성
                if (propertyReader.TryReadProperty(propertyCreateData, out EventField? propertyData) == false) {
                    JohwaLogger.Log($"프로퍼티 생성에 실패했습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);

                    continue;
                }

                result.Add(propertyData);
            }
        }
        finally
        {
            // 프로퍼티 이름 버퍼 반납
            ArrayPool<byte>.Shared.Return(propertyNameBuffer);
        }
        
        return result;

        static bool CreateProperty(EventFieldCreateData createData, [NotNullWhen(true)] out EventField? property)
        {
            if (createData.descriptor.isFieldTypeEventProperty)
            {
                Type fieldType = createData.descriptor.fieldInfo.FieldType;

                // 타입 체크
                if (fieldType.IsAbstract) {
                    JohwaLogger.Log($"EventPropertyData ({fieldType})는 추상 클래스일 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);

                    property = null;
                    return false;
                }

                // 프로퍼티 리더
                EventPropertyReader? reader = EventPropertyReader.GetInstance(fieldType);
                if (reader == null) {
                    // 리더 생성
                    JohwaLogger.Log($"프로퍼티 리더를 찾을 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);

                    property = null;
                    return false;
                }

                // EventProperty 생성
                reader.TryReadProperty(createData, out property);
                if (property == null) {
                    JohwaLogger.Log($"EventPropertyData ({fieldType}) 생성에 실패하였습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    return false;
                }

                createData.descriptor.fieldInfo.SetValue(createData.descriptor.group, property);
                return true;
            }
            else
            {
                EventPropertyReader? reader = EventPropertyReader.GetInstance(createData.descriptor.fieldInfo.FieldType);
                if (reader == null) {
                    JohwaLogger.Log($"({createData.descriptor.fieldInfo.FieldType})EventPropertyReader를 얻을 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);

                    property = null;
                    return false;
                }

                return reader.TryReadProperty(createData, out property);
            }
        }
    }

    #endregion

    public EventDataContainer(ReadOnlyMemory<byte> data)
    {
        this.data = data;
        this.metadata = GetMetadata();
        this.fieldSet = CreatePropertySet();
    }

    public T GetValue<T>(string propertyName)
        where T : EventField
    {
        
    }
}