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
    #region Static

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

    #endregion


    #region Instance

    public EventDataDescriptorDictionary dataDescriptorDictionary = new();
    public EventFieldDescriptor[] fieldDescriptorArray;
    public EventPropertyDescriptor[] propertyDescriptorArray;
    public EventDataGroupDescriptor[] dataGroupDescriptorArray;

    public EventDataContainerMetadata(Type dataType)
    {
        this.propertyDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(dataType);
    }

    public IEnumerable<EventDataDescriptor> GetEventDataDescriptorEnumerable()
    {
        // 필드
        for (int i = 0; i < fieldDescriptorArray.Length; i++)
        {
            yield return fieldDescriptorArray[i];
        }

        // 프로퍼티
        for (int i = 0; i < propertyDescriptorArray.Length; i++)
        {
            yield return propertyDescriptorArray[i];
        }

        // 그룹
        for (int i = 0; i < dataGroupDescriptorArray.Length; i++)
        {
            EventDataGroupDescriptor descriptor = dataGroupDescriptorArray[i];
            foreach (EventDataDescriptor dataDescriptor in descriptor.GetEventDataDescriptorEnumerable())
            {
                yield return dataDescriptor;
            }
        }
    }


    #endregion
}

public abstract class EventDataContainer : IEventDataGroup
{
    #region Instance

    public EventDataContainerMetadata metadata;
    public EventFieldSet fieldSet;
    public EventPropertySet propertySet;
    public EventDataGroupSet dataGroupSet;

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
                ReadOnlyMemory<byte> propertyJsonData = jsonReader.ReadAndSliceValue(data);

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
        this.fieldSet = CreatePropertySet(data,  out fieldSet, out propertySet, out dataGroupSet);
    }


    public T GetValue<T>(string propertyName)
        where T : EventField
    {
        
    }

    public void CreateData(
        out EventFieldSet fieldSet, out EventPropertySet propertySet, out EventDataGroupSet dataGroupSet)
    {
        // 그룹 데이터
        dataGroupSet = new EventDataGroupSet(metadata.dataGroupDescriptorArray);

        // 필드 데이터
        fieldSet = new EventFieldSet(metadata.fieldDescriptorArray);

        ValueSet<EventFieldDescriptor, EventFieldDescriptor.NameId> fieldDescriptorSet = new (
            new ReadOnlyMemory<EventFieldDescriptor>(metadata.fieldDescriptorArray), 
            stackalloc ValueSet<EventFieldDescriptor, EventFieldDescriptor.NameId>.LinkedListNode[metadata.fieldDescriptorArray.Length]
        );

        // 프로퍼티 데이터
        propertySet = new EventPropertySet(metadata.propertyDescriptorArray);

        ValueSet<EventPropertyDescriptor, EventPropertyDescriptor.NameId> propertyDescriptorSet = new (
            new ReadOnlyMemory<EventPropertyDescriptor>(metadata.propertyDescriptorArray), 
            stackalloc ValueSet<EventPropertyDescriptor, EventPropertyDescriptor.NameId>.LinkedListNode[metadata.propertyDescriptorArray.Length]
        );


        // Json 읽기
        ReadOnlySpan<byte> dataSpan = data.Span;
        Utf8JsonReader jsonReader = new(dataSpan);

        while (jsonReader.Read())
        {
            // 이름만 필터링
            if (jsonReader.TokenType != JsonTokenType.PropertyName) continue;

            // 데이터 이름
            ReadOnlySpan<byte> jsonDataNameSpan = jsonReader.ValueSpan;


            // 프로퍼티 탐색
            EventPropertyDescriptor.NameId propertyNameId = new (jsonDataNameSpan);

            if (propertyDescriptorSet.TryExtractValue(propertyNameId, 
                out EventPropertyDescriptor? propertyDescriptor, EventPropertyDescriptor.NameId.IsNameMatch)) 
            {
                // 불가능한 오류 (컴파일러 안심)
                if (propertyDescriptor == null) throw new NullReferenceException("불가능한 오류");

                // 값으로 이동
                jsonReader.Read();

                // 값 읽기
                JsonTokenType valueTokenType = jsonReader.TokenType;
                ReadOnlyMemory<byte> jsonData = jsonReader.ReadAndSliceValue(in data);

                // 프로퍼티 생성
                EventProperty property = new EventProperty(this, propertyDescriptor, jsonData, valueTokenType);
                
                // 세트에 추가
                propertySet.Add(property);
                continue;
            }


            // 필드 탐색
            EventFieldDescriptor.NameId fieldNameId = new (jsonDataNameSpan);

            if (fieldDescriptorSet.TryExtractValue(fieldNameId, 
                out EventFieldDescriptor? fieldDescriptor, EventFieldDescriptor.NameId.IsNameMatch)) 
            {
                // 불가능한 오류 (컴파일러 안심)
                if (fieldDescriptor == null) throw new NullReferenceException("불가능한 오류");
                
                // 값으로 이동
                jsonReader.Read();

                // 값 읽기
                JsonTokenType valueTokenType = jsonReader.TokenType;
                ReadOnlyMemory<byte> jsonData = jsonReader.ReadAndSliceValue(in data);

                // 필드 생성 정보
                EventField.CreateData createData = new (this, fieldDescriptor, jsonData, jsonReader.TokenType);

                // 필드 생성
                EventField? field = EventField.CreateInstance(createData);
                if (field == null) continue;
                
                // 세트에 추가
                fieldSet.Add(field);
            }


            // 데이터 그룹 탐색
            EventDataGroupDescriptor.NameId dataGroupNameId = new (jsonDataNameSpan);
        }
        


        for (int i = 0; i < fieldDescriptorArray.Length; i++)
        {
            EventFieldDescriptor descriptor = fieldDescriptorArray[i];

            // 생성 정보
            EventField.CreateData createData = new (declaringObject, groupType, descriptor, data, tokenType);

            // 필드 생성
            EventField? field = EventField.CreateInstance(createData);
            if (field == null) {
                JohwaLogger.Log($"EventFieldData ({descriptor.fieldInfo.FieldType}) 생성에 실패하였습니다.",
                    severity: LogSeverity.Warning, stackTrace: true);
                Array.Resize(ref fieldSet, fieldSet.Length - 1);
                i--;
                continue;
            }
            fieldSet[i] = field;
        }

        return fieldSet;
    }
    
}