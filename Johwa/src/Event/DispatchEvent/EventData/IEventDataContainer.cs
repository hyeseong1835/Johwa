using System.Buffers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Common.Debug;
using Johwa.Common.Extension.System.Text.Json;
using Johwa.Event.Data.PropertyReaders;

namespace Johwa.Event.Data;

public interface IEventDataContainerMetadata : IEventDataGroupMetadata
{

}

public interface IEventDataContainer : IEventDataGroup
{
    #region Static

    #endregion


    #region Instance

    #region 재정의

    IEventDataGroupMetadata IEventDataGroup.GroupMetadata => ContainerMetadata;

    #endregion

    public IEventDataContainerMetadata ContainerMetadata { get; }
    public ReadOnlyMemory<byte> Data { get; }

    public List<EventProperty> CreateProperties()
    {
        IEventDataContainerMetadata metadata = ContainerMetadata;

        EventPropertyAttribute[] propertyDescriptorArray = metadata.PropertyDescriptorArray;

        ReadOnlyMemory<byte> dataMemory = Data;
        ReadOnlySpan<byte> dataSpan = dataMemory.Span;

        // 결과 초기화
        List<EventProperty> result = new(metadata.PropertyDescriptorArray.Length);

        // Json 읽기
        Utf8JsonReader reader = new(dataSpan);
        
        // 노드 버퍼 (스택)
        Span<ValueSet<EventPropertyAttribute, ReadOnlyMemory<byte>>.LinkedListNode> nodeBuffer 
            = stackalloc ValueSet<EventPropertyAttribute, ReadOnlyMemory<byte>>.LinkedListNode[propertyDescriptorArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ValueSet<EventPropertyAttribute, ReadOnlyMemory<byte>> propertyDescriptorSet 
            = new(new ReadOnlyMemory<EventPropertyAttribute>(propertyDescriptorArray), nodeBuffer);

        // 프로퍼티 이름 버퍼 대여
        byte[] propertyNameBuffer = ArrayPool<byte>.Shared.Rent(64);
        try
        {
            // 읽기
            while (reader.Read())
            {
                // 프로퍼티 이름이 아닐 경우 무시
                if (reader.TokenType != JsonTokenType.PropertyName) continue;

                // 프로퍼티 이름 복사
                ReadOnlySpan<byte> valueSpan = reader.ValueSpan;
                valueSpan.CopyTo(propertyNameBuffer);

                // 프로퍼티 메타데이터 탐색
                EventPropertyAttribute? propertyDescriptor;
                ReadOnlyMemory<byte> propertyName = new(propertyNameBuffer, 0, valueSpan.Length);
                if (propertyDescriptorSet.TryExtractValue(propertyName, out propertyDescriptor, EventPropertyAttribute.IsNameMatch) == false) {
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
                reader.Read();

                // 프로퍼티 데이터 타입
                JsonTokenType propertyDataTokenType = reader.TokenType;
                
                // 프로퍼티 데이터 자르기
                ReadOnlyMemory<byte> propertyJsonData = reader.ReadAndSliceToken(dataMemory);

                // 프로퍼티 생성
                EventProperty? propertyData = ReadPropertyData(this, propertyJsonData, propertyDataTokenType);
                result.Add(propertyData);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventPropertyAttribute propertyDescriptor in propertyDescriptorSet.GetEnumerable())
            {
                // 파라미터로 넘겨줄 데이터 : 이름이 비었으면 전체 데이터 전달
                ReadOnlyMemory<byte> propertyDataMemory = (propertyDescriptor.name == null)? dataMemory : ReadOnlyMemory<byte>.Empty;

                // 프로퍼티 생성
                EventProperty propertyData = propertyDescriptor.ReadPropertyData(this, propertyDataMemory, JsonTokenType.None);
                result.Add(propertyData);
            }
        }
        finally
        {
            // 프로퍼티 이름 버퍼 반납
            ArrayPool<byte>.Shared.Return(propertyNameBuffer);
        }
        
        return result;

        /// <summary>
        /// Descriptor의
        /// </summary>
        static bool SetField(IEventDataGroup propertyGroup, EventPropertyAttribute propertyDescriptor, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            // FieldInfo
            FieldInfo? fieldInfo = propertyDescriptor.fieldInfo;
            if (fieldInfo == null) {
                JohwaLogger.Log("EventPropertyAttribute가 초기화되지 않았습니다.",
                    severity: LogSeverity.Warning, stackTrace: true);
                return false;
            }


            // 필드 타입이 EventProperty를 상속하는 클래스인 경우
            if (propertyDescriptor.isFieldTypeEventProperty) 
            {
                return InitEventProperty(propertyGroup, propertyDescriptor, data, tokenType, fieldInfo);
            }
            // EventProperty가 아닌 타입인 경우
            else
            {
                // 프로퍼티 리더 로드
                EventPropertyReader? reader = EventPropertyReader.GetInstance(fieldType);
                if (reader == null) {
                    JohwaLogger.Log($"필드 ({propertyGroup.GetType().Name}/{fieldType})를 지원하는 EventPropertyReader를 찾을 수 없습니다.",
                        severity: LogSeverity.Error, stackTrace: true);
                    return false;
                }

                // 필드 읽기
                if (reader.TryReadProperty(fieldInfo, propertyGroup, data, tokenType) == false) {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 필드 타입이 EventProperty를 상속하는 클래스
            /// 필드 타입이 추상 -> EventPropertyData ({fieldType})는 추상 클래스일 수 없습니다.
            /// 이벤트 프로퍼티 생성 실패 -> EventPropertyData ({fieldType}) 생성에 실패하였습니다.
            /// 메타데이터 생성 실패 -> EventPropertyData ({fieldType}) 메타데이터 생성에 실패하였습니다.
            /// <summary/>
            static bool InitEventProperty(IEventDataGroup propertyGroup, EventPropertyAttribute propertyDescriptor, ReadOnlyMemory<byte> data, JsonTokenType tokenType,
                FieldInfo fieldInfo)
            {
                Type fieldType = fieldInfo.FieldType;

                // 타입 체크
                if (fieldType.IsAbstract) {
                    JohwaLogger.Log($"EventPropertyData ({fieldType})는 추상 클래스일 수 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    return false;
                }

                // EventProperty 생성
                EventProperty? property = (EventProperty?)Activator.CreateInstance(fieldType);
                if (property == null) {
                    JohwaLogger.Log($"EventPropertyData ({fieldType}) 생성에 실패하였습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    return false;
                }

                EventPropertyMetadata? dataMetadata = property.CreateMetadata(fieldInfo);
                if (dataMetadata == null) {
                    JohwaLogger.Log($"EventPropertyData ({fieldType}) 메타데이터 생성에 실패하였습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    return false;
                }
                property.Init(new EventProperty.Info(propertyDescriptor, data));

                fieldInfo.SetValue(propertyGroup, property);
                return true;
            }   
        }
    }

    #endregion
}