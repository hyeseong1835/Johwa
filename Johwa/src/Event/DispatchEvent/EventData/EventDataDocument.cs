using System.Buffers;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Common.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;
    public int propertyCount;
    public Type eventDataType;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;
        propertyDescriptorArray = IEventDataContainer.LoadDescriptors(eventDataType).ToArray();
    }
}

public abstract class EventDataDocument : IEventDataContainer
{
    #region 재정의

    IEnumerable<EventPropertyData> IEventDataContainer.GetPropertyDataEnumerable()
    {
        for (int i = 0; i < propertyData.Count; i++)
        {
            EventPropertyData property = propertyData[i];
            yield return property;
        }
    }
    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        foreach (IDisposable property in propertyData)
        {
            property.Dispose();
        }
    }
    
    #endregion


    #region 필드

    public readonly EventDataDocumentMetadata metadata;
    public ReadOnlyMemory<byte> data;
    public readonly List<EventPropertyData> propertyData;

    #endregion


    #region 생성자

    public EventDataDocument(ReadOnlyMemory<byte> data)
    {
        this.data = data;
        this.metadata = GetMetadata();
        this.propertyData = CreateProperties();
    }

    #endregion
    

    #region 추상
    
    protected abstract EventDataDocumentMetadata GetMetadata();

    #endregion


    #region 메서드

    List<EventPropertyData> CreateProperties()
    {
        List<EventPropertyData> result = new(metadata.propertyDescriptorArray.Length);

        // Json 읽기
        Utf8JsonReader reader = new(data.Span);
        
        // 노드 버퍼 (스택)
        Span<ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>>.LinkedListNode> nodeBuffer 
            = stackalloc ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>>.LinkedListNode[metadata.propertyDescriptorArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventPropertyDescriptorAttribute>(metadata.propertyDescriptorArray), nodeBuffer);

        // 프로퍼티 이름 버퍼 대여
        byte[] propertyNameBuffer = ArrayPool<byte>.Shared.Rent(64);
        try
        {
            // 읽기
            while (reader.Read())
            {
                // 프로퍼티 이름이 아닐 경우 무시
                if (reader.TokenType != JsonTokenType.PropertyName) continue;

                // 프로퍼티 메타데이터 탐색
                EventPropertyDescriptorAttribute? propertyDescriptor;
                reader.ValueSpan.CopyTo(propertyNameBuffer);
                ReadOnlyMemory<byte> propertyName = new(propertyNameBuffer, 0, reader.ValueSpan.Length);
                if (propertyMetadataSet.TryExtractValue(propertyName, out propertyDescriptor, EventPropertyDescriptorAttribute.IsNameMatch) == false) {
                    // 프로퍼티 메타데이터를 찾을 수 없을 경우 예외 발생
                    throw new InvalidOperationException("오류");
                }

                // 불가능한 오류 (컴파일러 안심)
                if (propertyDescriptor == null) {
                    throw new InvalidOperationException("오류");
                }

                // 값으로 이동
                reader.Read();

                // 프로퍼티 데이터 타입
                JsonTokenType propertyDataTokenType = reader.TokenType;
                
                // 프로퍼티 데이터 자르기
                ReadOnlyMemory<byte> propertyJsonData = reader.ReadAndSliceToken(data);

                // 프로퍼티 생성
                EventPropertyData propertyData = propertyDescriptor.CreatePropertyData(this, propertyJsonData, propertyDataTokenType);
                result.Add(propertyData);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventPropertyDescriptorAttribute propertyDescriptor in propertyMetadataSet.GetEnumerable())
            {
                // 파라미터로 넘겨줄 데이터 : 이름이 비었으면 전체 데이터 전달
                ReadOnlyMemory<byte> data = (propertyDescriptor.name.IsEmpty)? this.data : ReadOnlyMemory<byte>.Empty;

                // 프로퍼티 생성
                EventPropertyData propertyData = propertyDescriptor.CreatePropertyData(this, data, JsonTokenType.None);
                result.Add(propertyData);
            }
        }
        finally
        {
            // 프로퍼티 이름 버퍼 반납
            ArrayPool<byte>.Shared.Return(propertyNameBuffer);
        }
        
        return result;
    }

    #endregion
}