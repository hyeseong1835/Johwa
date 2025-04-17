using System.Text.Json;
using System.Buffers;
using System.Reflection;
using Johwa.Common.Debug;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public interface IEventDataGroup
{
    public static void CreateDescriptors(Type groupType, ref List<EventFieldDescriptor> fieldDescriptors, ref List<EventPropertyDescriptor> propertyDescriptors)
    {
        // 필드 정보
        FieldInfo[] fieldInfoArray = groupType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fieldInfoArray.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfoArray[i];

            // EventFieldDescriptorAttribute 로드
            EventFieldAttribute? propertyAttribute = fieldInfo.GetCustomAttribute<EventFieldAttribute>();
            if (propertyAttribute != null) {
                EventFieldDescriptor descriptor = propertyAttribute.CreateDescriptor(fieldInfo, fieldInfo.FieldType.IsSubclassOf(typeof(EventField)));
                fieldDescriptors.Add(descriptor);
                continue;
            }

            // EventDataGroupDescriptorAttribute 로드
            EventDataGroupAttribute? propertyGroupAttribute = fieldInfo.GetCustomAttribute<EventDataGroupAttribute>();
            if (propertyGroupAttribute != null) {
                CreateDescriptors(fieldInfo.FieldType, ref fieldDescriptors, ref propertyDescriptors);
                continue;
            }
        }

        // 프로퍼티 정보
        PropertyInfo[] propertyInfoArray = groupType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < propertyInfoArray.Length; i++)
        {
            PropertyInfo propertyInfo = propertyInfoArray[i];

            // EventPropertyDescriptorAttribute 로드
            EventPropertyAttribute? propertyAttribute = propertyInfo.GetCustomAttribute<EventPropertyAttribute>();
            if (propertyAttribute != null) {
                EventPropertyDescriptor descriptor = propertyAttribute.CreateDescriptor(propertyInfo, propertyInfo.PropertyType.IsSubclassOf(typeof(EventField)));
                propertyDescriptors.Add(descriptor);
                continue;
            }
        }
    }

    public static EventField[] CreateFieldSet(Type groupType, ReadOnlyMemory<byte> data, 
        EventFieldDescriptor[] fieldDescriptorArray)
    {
        EventField[] fieldSet = new EventField[fieldDescriptorArray.Length];

        ReadOnlySpan<byte> dataSpan = data.Span;
        Utf8JsonReader jsonReader = new(dataSpan);
        Span<ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>>.LinkedListNode> nodeBuffer 
            = stackalloc ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>>.LinkedListNode[fieldDescriptorArray.Length];

        ValueSet<EventFieldDescriptor, ReadOnlyMemory<byte>> fieldDescriptorSet
            = new (new ReadOnlyMemory<EventFieldDescriptor>(fieldDescriptorArray), nodeBuffer);

        byte[] nameBuffer = ArrayPool<byte>.Shared.Rent(64);

        while (jsonReader.Read())
        {
            // 오브젝트 스킵
            if (jsonReader.TokenType == JsonTokenType.StartObject) {
                jsonReader.Skip(); 
                continue;
            }

            // 배열 스킵
            if (jsonReader.TokenType == JsonTokenType.StartArray) {
                jsonReader.Skip(); 
                continue;
            }

            // 프로퍼티 이름이 아닐 경우 무시
            if (jsonReader.TokenType != JsonTokenType.PropertyName) continue;

            // 프로퍼티 이름 복사
            ReadOnlySpan<byte> valueSpan = jsonReader.ValueSpan;
            valueSpan.CopyTo(nameBuffer);

            ReadOnlyMemory<byte> nameMemory = new(nameBuffer, 0, valueSpan.Length);

            // 필드 탐색
            if (fieldDescriptorSet.TryGetValue(nameMemory, out EventFieldDescriptor? descriptor, EventFieldDescriptor.IsNameMatch) == false) continue;

            // 필드 생성 정보
            EventField.CreateData createData = new (groupType, descriptor, data, jsonReader.TokenType);

            // 필드 생성
            EventField? field = EventField.CreateField(createData);
            if (field == null) {
                JohwaLogger.Log($"EventFieldData ({descriptor.fieldInfo.FieldType}) 생성에 실패하였습니다.",
                    severity: LogSeverity.Warning, stackTrace: true);
                Array.Resize(ref fieldSet, fieldSet.Length - 1);
                continue;
            }
            
            fieldSet[fieldSet.Length - 1] = field;
        }


        for (int i = 0; i < fieldDescriptorArray.Length; i++)
        {
            EventFieldDescriptor descriptor = fieldDescriptorArray[i];

            // 생성 정보
            EventField.CreateData createData = new (groupType, descriptor, data, tokenType);

            // 필드 생성
            EventField? field = EventField.CreateField(createData);
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
    public static EventProperty[] CreatePropertySet(Type groupType, ReadOnlyMemory<byte> data, JsonTokenType tokenType,
        EventPropertyDescriptor[] propertyDescriptorArray)
    {
        EventProperty[] propertySet = new EventProperty[propertyDescriptorArray.Length];

        for (int i = 0; i < propertyDescriptorArray.Length; i++)
        {
            EventPropertyDescriptor descriptor = propertyDescriptorArray[i];

            // 생성 정보
            EventProperty.CreateData createData = new (groupType, descriptor, data, tokenType);

            // 프로퍼티 생성
            EventProperty? property = EventProperty.CreateProperty(createData);
            if (property == null) {
                JohwaLogger.Log($"EventPropertyData ({descriptor.fieldInfo.FieldType}) 생성에 실패하였습니다.",
                    severity: LogSeverity.Warning, stackTrace: true);
                Array.Resize(ref propertySet, propertySet.Length - 1);
                i--;
                continue;
            }
            propertySet[i] = property;
        }

        return propertySet;
    }
}