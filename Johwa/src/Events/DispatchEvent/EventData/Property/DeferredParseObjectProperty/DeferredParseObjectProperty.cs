using System.Buffers;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class DeferredParseObjectMetaData : EventDataMetadata
{
    public static Dictionary<Type, Func<DeferredParseObjectMetaData, ReadOnlyMemory<byte>, DeferredParseObjectProperty>> propertyConstructorDictionary = new();

    public override EventDataAttribute Attribute => attribute;
    public readonly DeferredParseObjectAttribute attribute;

    public readonly EventDataMetadata[] propertyMetadataArray;
    public readonly Func<DeferredParseObjectMetaData, ReadOnlyMemory<byte>, DeferredParseObjectProperty> createPropertyFunc;

    public DeferredParseObjectMetaData(DeferredParseObjectAttribute attribute, 
        FieldInfo fieldInfo) : base(fieldInfo)
    {
        this.attribute = attribute;

        // 프로퍼티 메타데이터 배열 로드
        propertyMetadataArray = EventProperty.LoadMetadata(fieldInfo.FieldType);

        // 프로퍼티 생성자
        Func<DeferredParseObjectMetaData, ReadOnlyMemory<byte>, DeferredParseObjectProperty>? createPropertyFunc;
        if (propertyConstructorDictionary.TryGetValue(fieldInfo.FieldType, out createPropertyFunc) == false)
        {
            ConstructorInfo? constructorInfo = fieldInfo.FieldType.GetConstructor([ typeof(ReadOnlyMemory<byte>) ]);
            if (constructorInfo == null)
                throw new InvalidOperationException("오류");
            
            createPropertyFunc = Expression.Lambda<Func<DeferredParseObjectMetaData, ReadOnlyMemory<byte>, DeferredParseObjectProperty>>(Expression.New(constructorInfo)).Compile();
        }
        this.createPropertyFunc = createPropertyFunc;
    }
    public override void InitProperty(object obj, ReadOnlyMemory<byte> data, JsonTokenType tokenType)
    {
        if (tokenType == JsonTokenType.Null)
            return;

        if (tokenType != JsonTokenType.StartObject)
            throw new InvalidOperationException("오류");
        
        Type fieldType = fieldInfo.FieldType;

        if (fieldType.IsAbstract)
            throw new InvalidOperationException("오류");

        if (fieldType.IsSubclassOf(typeof(DeferredParseObjectProperty)) == false)
            throw new InvalidOperationException("오류");

        // 오브젝트 생성
        DeferredParseObjectProperty property = createPropertyFunc.Invoke(this, data);
        property.Init();

        fieldInfo.SetValue(obj, property);
        
        // Json 읽기
        Utf8JsonReader reader = new Utf8JsonReader(data.Span);
        
        // 노드 버퍼 (스택)
        Span<ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>>.LinkedListNode> nodeBuffer 
            = stackalloc ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>>.LinkedListNode[propertyMetadataArray.Length];

        // 프로퍼티 메타데이터 탐색을 위한 세트 생성
        ReadOnlyValueSet<EventDataMetadata, ValueTuple<byte[], int>> propertyMetadataSet 
            = new(new ReadOnlyMemory<EventDataMetadata>(propertyMetadataArray), nodeBuffer, IsMatchMetaData);

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

public class DeferredParseObjectAttribute : EventDataAttribute
{
    public DeferredParseObjectAttribute(
        string name, bool isOptional = false) : base(name, isOptional) { }
    public override EventDataMetadata CreateMetadata(FieldInfo fieldInfo)
    {
        return new DeferredParseObjectMetaData(this, fieldInfo);
    }
}
public abstract class DeferredParseObjectProperty : EventProperty
{
    public DeferredParseObjectProperty(
        ReadOnlyMemory<byte> data) : base(data) { }

    public abstract void Init();
}