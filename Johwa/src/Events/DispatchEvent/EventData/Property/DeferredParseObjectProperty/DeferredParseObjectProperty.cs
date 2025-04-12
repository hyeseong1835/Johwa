using System.Buffers;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseObjectMetaData : EventPropertyMetadata
{
    // 필드
    public readonly int minPropertyCount;

    // 재정의
    public override EventPropertyData CreatePropertyData(IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        => DeferredParseObjectPropertyData.GetInstance(this, container, data, tokenType);

    // 필드
    public readonly EventPropertyMetadata[] propertyMetadataArray;

    // 생성자
    public DeferredParseObjectMetaData(DeferredParseObjectAttribute attribute, 
        FieldInfo fieldInfo) : base(fieldInfo)
    {
        this.attribute = attribute;

        // 프로퍼티 메타데이터 배열 로드
        propertyMetadataArray = LoadMetadata(fieldInfo.FieldType).ToArray();
        
        // 프로퍼
        minPropertyCount = 0;
        for (int i = 0; i < propertyMetadataArray.Length; i++)
        {
            EventPropertyMetadata propertyMetadata = propertyMetadataArray[i];
            if (propertyMetadata.Attribute.isOptional == false) {
                minPropertyCount++;
            }
        }
    }
}

public class DeferredParseObjectAttribute : EventPropertyDescriptorAttribute
{
    // 재정의
    public override Type PropertyMetaDataType => typeof(DeferredParseObjectMetaData);
    public override EventPropertyMetadata CreateMetadata(FieldInfo fieldInfo)
        => new DeferredParseObjectMetaData(this, fieldInfo);

    // 생성자
    public DeferredParseObjectAttribute(
        string name, bool isOptional = false, bool isNullable = false) : base(name, isOptional, isNullable) { }
}
public abstract class DeferredParseObjectPropertyData : EventPropertyData, IEventDataContainer
{
    #region Static

    static readonly Dictionary<Type, Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData>> instanceConstructorDictionary = new();
    
    static Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData> GetInstanceConstructor(Type type)
    {
        Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData>? result;
        if (instanceConstructorDictionary.TryGetValue(type, out result) == false)
        {
            ConstructorInfo? constructorInfo = type.GetConstructor([ typeof(DeferredParseObjectMetaData), typeof(IEventDataContainer), typeof(ReadOnlyMemory<byte>), typeof(JsonTokenType) ]);
            if (constructorInfo == null)
                throw new InvalidOperationException("오류");

            NewExpression newExpression = Expression.New(constructorInfo);
            Expression<Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData>> expression 
                = Expression.Lambda<Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData>>(newExpression);
            result = expression.Compile();
            instanceConstructorDictionary.Add(type, result);
        }
        return result;
    }
    public static DeferredParseObjectPropertyData GetInstance(DeferredParseObjectMetaData metaData, IEventDataContainer container, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        Func<DeferredParseObjectMetaData, IEventDataContainer, ReadOnlyMemory<byte>, JsonTokenType, DeferredParseObjectPropertyData> constructor
            = GetInstanceConstructor(metaData.fieldInfo.FieldType);

        return constructor.Invoke(metaData, container, data, tokenType);
    }
    
    #endregion


    #region Instance

    #region 재정의
    
    public override EventPropertyDescriptorAttribute Descriptor => DeferredParseObjectAttribute;

    IEnumerable<EventPropertyData> IEventDataContainer.GetPropertyDataEnumerable()
    {
        for (int i = 0; i < propertyDataList.Count; i++)
        {
            EventPropertyData property = propertyDataList[i];
            yield return property;
        }
    }

    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        for (int i = 0; i < propertyDataList.Count; i++)
        {
            IDisposable property = propertyDataList[i];
            property.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    #endregion

    #region 필드

    public abstract EventPropertyDescriptorAttribute DeferredParseObjectAttribute { get; }
    public readonly List<EventPropertyData> propertyDataList;

    #endregion

    // 생성자
    public DeferredParseObjectPropertyData(DeferredParseObjectMetaData metaData, IEventDataContainer container, 
        ReadOnlyMemory<byte> data, JsonTokenType tokenType) : base(metaData, data) 
    {
        this.data = data;
        this.propertyDataList = new List<EventPropertyData>(metaData.attribute.minPropertyCount);

        // 토큰 타입 검사
        if (tokenType == JsonTokenType.None)
        {
            if (metaData.attribute.isOptional == false)
                throw new InvalidOperationException("오류");
        }
        if (tokenType == JsonTokenType.Null)
        {
            if (metaData.attribute.isNullable == false)
                throw new InvalidOperationException("오류");
        }
        if (tokenType != JsonTokenType.StartObject)
            throw new InvalidOperationException("오류");
        
        // 타입 검사
        Type fieldType = metaData.fieldInfo.FieldType;
        if (fieldType.IsAbstract)
            throw new InvalidOperationException("오류");

        if (fieldType.IsSubclassOf(typeof(DeferredParseObjectPropertyData)) == false)
            throw new InvalidOperationException("오류");
        
        // Json 읽기
        Utf8JsonReader reader = new Utf8JsonReader(data.Span);
        
        // 노드 버퍼 (스택)
        Span<ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>>.LinkedListNode> nodeBuffer 
            = stackalloc ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>>.LinkedListNode[metaData.propertyMetadataArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ReadOnlyValueSet<EventPropertyMetadata, ValueTuple<byte[], int>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventPropertyMetadata>(metaData.propertyMetadataArray), nodeBuffer);

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

                // 프로퍼티 데이터 생성
                EventPropertyData propertyData = propertyMetadata.CreatePropertyData(this, propertyJsonData, propertyDataTokenType);
                propertyDataList.Add(propertyData);
            }

            // Json에서 찾지 못한 프로퍼티 초기화
            foreach (EventPropertyMetadata propertyMetadata in propertyMetadataSet.GetEnumerable())
            {
                EventPropertyData propertyData = base.propertyMetadata.CreatePropertyData(this, ReadOnlyMemory<byte>.Empty, JsonTokenType.None);
                propertyDataList.Add(propertyData);
            }
        }
        finally
        {
            // 프로퍼티 이름 버퍼 반납
            ArrayPool<byte>.Shared.Return(propertyNameBuffer);
        }
    }

    public abstract void Init();

    #endregion
}