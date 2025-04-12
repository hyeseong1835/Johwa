using System.Buffers;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public readonly EventPropertyMetadata[] propertyMetadataArray;
    public int propertyCount;
    public Type eventDataType;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;

        propertyMetadataArray = EventPropertyMetadata.LoadMetadata(eventDataType).ToArray();
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
        this.propertyData = CreateProperty();
    }

    #endregion
    

    #region 추상
    
    protected abstract EventDataDocumentMetadata GetMetadata();

    #endregion


    #region 메서드

    List<EventPropertyData> CreateProperty()
    {
        List<EventPropertyData> result = new(metadata.propertyMetadataArray.Length);

        // Json 읽기
        Utf8JsonReader reader = new(data.Span);
        
        // 노드 버퍼 (스택)
        Span<ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>>.LinkedListNode> nodeBuffer 
            = stackalloc ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>>.LinkedListNode[metadata.propertyMetadataArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventPropertyMetadata>(metadata.propertyMetadataArray), nodeBuffer);

        // 프로퍼티 이름 버퍼 대여
        byte[] propertyNameBuffer = ArrayPool<byte>.Shared.Rent(64);
        try
        {
            // 읽기
            while (reader.Read())
            {
                // 프로퍼티 이름이 아닐 경우 무시
                if (reader.TokenType != JsonTokenType.PropertyName) continue;
                
                // 프로퍼티 이름을 버퍼에 복사
                reader.ValueSpan.CopyTo(propertyNameBuffer);
                ValueTuple<byte[], int> nameData = (propertyNameBuffer, reader.ValueSpan.Length);

                // 프로퍼티 메타데이터 탐색
                EventPropertyMetadata? propertyMetadata;
                if (propertyMetadataSet.TryExtractValue(nameData, out propertyMetadata, EventPropertyMetadata.IsNameMatchMetaData) == false) {
                    // 프로퍼티 메타데이터를 찾을 수 없을 경우 예외 발생
                    throw new InvalidOperationException("오류");
                }

                // 불가능한 오류 (컴파일러 안심)
                if (propertyMetadata == null){
                    throw new InvalidOperationException("오류");
                }

                // 값으로 이동
                reader.Read();

                // 프로퍼티 데이터 타입
                JsonTokenType propertyDataTokenType = reader.TokenType;
                
                // 프로퍼티 데이터 자르기
                ReadOnlyMemory<byte> propertyJsonData = reader.ReadAndSliceToken(data);

                // 프로퍼티 생성
                EventPropertyData propertyData = propertyMetadata.CreatePropertyData(this, propertyJsonData, propertyDataTokenType);
                result.Add(propertyData);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventPropertyMetadata propertyMetadata in propertyMetadataSet.GetEnumerable())
            {
                // 프로퍼티 생성
                EventPropertyData propertyData = propertyMetadata.CreatePropertyData(this, default, JsonTokenType.None);
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