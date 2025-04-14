using System.Buffers;
using System.Reflection;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Common.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public interface IEventDataGroupMetadata
{
    #region Static

    public static EventPropertyDescriptorAttribute[] LoadPropertyDescriptors(Type groupType)
    {
        List<EventPropertyDescriptorAttribute> result = new();

        // 필드 정보 가져오기
        FieldInfo[] fields = groupType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            // EventPropertyDescriptorAttribute 로드
            EventPropertyDescriptorAttribute? propertyDescriptor = field.GetCustomAttribute<EventPropertyDescriptorAttribute>();
            if (propertyDescriptor != null)
            {
                result.Add(propertyDescriptor);
                continue;
            }

            // EventPropertyGroupDescriptorAttribute 로드
            EventPropertyGroupAttribute? propertyGroupDescriptor = field.GetCustomAttribute<EventPropertyGroupAttribute>();
            if (propertyGroupDescriptor != null)
            {
                // 메타데이터 로드
                EventPropertyGroupMetadata metadata = EventPropertyGroupMetadata.GetInstance(groupType);
                result.AddRange(metadata.propertyDescriptorArray);
                continue;
            }
        }
        return result.ToArray();
    }

    #endregion

    public Type GroupType { get; }
    public EventPropertyDescriptorAttribute[] PropertyDescriptorArray { get; }
}
public interface IEventDataContainerMetadata : IEventDataGroupMetadata
{

}
public interface IEventDataGroup : IDisposable
{
    public IEventDataGroupMetadata GroupMetadata { get; }
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

    public List<EventPropertyData> CreateProperties()
    {
        IEventDataContainerMetadata metadata = ContainerMetadata;

        EventPropertyDescriptorAttribute[] propertyDescriptorArray = metadata.PropertyDescriptorArray;

        ReadOnlyMemory<byte> dataMemory = Data;
        ReadOnlySpan<byte> dataSpan = dataMemory.Span;

        List<EventPropertyData> result = new(metadata.PropertyDescriptorArray.Length);

        // Json 읽기
        Utf8JsonReader reader = new(dataSpan);
        
        // 노드 버퍼 (스택)
        Span<ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>>.LinkedListNode> nodeBuffer 
            = stackalloc ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>>.LinkedListNode[propertyDescriptorArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ValueSet<EventPropertyDescriptorAttribute, ReadOnlyMemory<byte>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventPropertyDescriptorAttribute>(propertyDescriptorArray), nodeBuffer);

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
                EventPropertyDescriptorAttribute? propertyDescriptor;
                ReadOnlyMemory<byte> propertyName = new(propertyNameBuffer, 0, valueSpan.Length);
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
                ReadOnlyMemory<byte> propertyJsonData = reader.ReadAndSliceToken(dataMemory);

                // 프로퍼티 생성
                EventPropertyData propertyData = propertyDescriptor.CreatePropertyData(this, propertyJsonData, propertyDataTokenType);
                result.Add(propertyData);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventPropertyDescriptorAttribute propertyDescriptor in propertyMetadataSet.GetEnumerable())
            {
                // 파라미터로 넘겨줄 데이터 : 이름이 비었으면 전체 데이터 전달
                ReadOnlyMemory<byte> propertyDataMemory = (propertyDescriptor.name.IsEmpty)? dataMemory : ReadOnlyMemory<byte>.Empty;

                // 프로퍼티 생성
                EventPropertyData propertyData = propertyDescriptor.CreatePropertyData(this, propertyDataMemory, JsonTokenType.None);
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