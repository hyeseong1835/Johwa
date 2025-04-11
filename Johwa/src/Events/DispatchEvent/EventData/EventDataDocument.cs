using System.Buffers;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public readonly EventDataMetadata[] propertyMetadataArray;
    public Type eventDataType;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;

        propertyMetadataArray = EventProperty.LoadMetadata(eventDataType);
    }
}

public abstract class EventDataDocument : IDisposable
{
    byte[]? data;
    
    public EventDataDocument(byte[] data)
    {
        this.data = data;
    }
    public void Dispose()
    {
        data = null;
    }
    public abstract void Init();

    public void InitProperty(EventDataDocumentMetadata metadata)
    {
        if (data == null)
            throw new InvalidOperationException("데이터를 참조할 수 없습니다.");

        // Json 읽기
        Utf8JsonReader reader = new Utf8JsonReader(data);
        
        // 노드 버퍼 (스택)
        Span<ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>>.LinkedListNode> nodeBuffer 
            = stackalloc ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>>.LinkedListNode[metadata.propertyMetadataArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventDataMetadata>(metadata.propertyMetadataArray), nodeBuffer, IsMatchMetaData);

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
                if (propertyMetadataSet.TryGetExtractValue(nameData, out EventDataMetadata? propertyMetadata) == false) {
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
                ReadOnlyMemory<byte> propertyData = reader.ReadAndSliceToken(data);

                // 프로퍼티 초기화
                propertyMetadata.InitProperty(this, propertyData, propertyDataTokenType);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventDataMetadata node in propertyMetadataSet.GetEnumerable())
            {
                node.InitProperty(this, default, JsonTokenType.None);
            }
        }
        finally
        {
            // 프로퍼티 이름 버퍼 반납
            ArrayPool<byte>.Shared.Return(propertyNameBuffer);
        }
    }
    static bool IsMatchMetaData(EventDataMetadata metadata, ValueTuple<byte[], int> nameData)
    {
        if (metadata.Attribute.name.Length != nameData.Item2)
            return false;
        
        for (int i = 0; i < metadata.Attribute.name.Length; i++)
        {
            if (nameData.Item1[i] != metadata.Attribute.name[i]) 
                return false;
        }
        
        return true;
    }
}