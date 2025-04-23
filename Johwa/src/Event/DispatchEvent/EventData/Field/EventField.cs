using Johwa.Common.Debug;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Johwa.Event.Data;

public abstract class EventField : EventData
{
    #region Object

    [AttributeUsage(AttributeTargets.Class)]
    protected class EventPropertyAttribute : Attribute
    {
        public Type fieldType;
        public EventPropertyAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }

    public struct EventFieldCreateData
    {
        public EventFieldDescriptor descriptor;
        public IEventDataGroup declaringGroup;
        public ReadOnlyMemory<byte> data;
        public JsonTokenType tokenType;

        public EventFieldCreateData(EventFieldDescriptor descriptor, IEventDataGroup declaringGroup,
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            this.descriptor = descriptor;
            this.declaringGroup = declaringGroup;
            this.data = data;
            this.tokenType = tokenType;
        }
        public EventFieldCreateData(EventFieldDescriptor descriptor, IEventDataGroup declaringGroup,
            ReadOnlyMemory<byte> data, JsonTokenType tokenType, Type dataType) : this(descriptor, declaringGroup, data, tokenType)
        {
            this.dataType = dataType;
        }
    }
    
    #endregion


    #region Static

    // 필드
    static Dictionary<Type, Func<EventFieldCreateData, EventField>> constructorDictionary = CreateConstructorDictionary();

    #region 메서드

    /// <summary>
    /// 캐시된 생성자를 찾거나 새로 캐시하여 실행합니다. <br/>
    /// <br/>
    /// [ 로그 ]
    /// EventField의 생성자를 찾을 수 없습니다. : {fieldType} <br/>
    /// </summary>
    /// <param name="createData"></param>
    /// <returns></returns>
    public static EventField? CreateInstance(EventFieldCreateData createData)
    {
        Func<EventFieldCreateData, EventField>? constructor = GetConstructor(createData.dataType);
        if (constructor == null) {
            JohwaLogger.Log($"EventField의 생성자를 찾을 수 없습니다. : {createData.dataType}",
                severity: LogSeverity.Warning, stackTrace: true);
            return null;
        }

        return constructor.Invoke(createData);
    }


    static Dictionary<Type, Func<EventFieldCreateData, EventField>> CreateConstructorDictionary()
    {
        Dictionary<Type, Func<EventFieldCreateData, EventField>> dictionary = new();
        
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            Type[] types = assembly.GetTypes();

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type type = types[typeIndex];
                
                // 클래스가 아니면 제외
                if (type.IsClass == false)
                    continue;
                
                // EventField 상속 클래스가 아니면 제외
                if (type.IsSubclassOf(typeof(EventField)) == false)
                    continue;

                // Attribute 로드
                EventFieldReaderAttribute? attribute = type.GetCustomAttribute<EventFieldReaderAttribute>();
                if (attribute == null) {
                    JohwaLogger.Log($"프로퍼티 ({type})에 EventFieldAttribute가 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    continue;
                }
                Type fieldType = attribute.fieldType;
                
                // 대상 타입을 정의하는 리더가 이미 존재하면
                if (dictionary.TryGetValue(fieldType, out Func<EventFieldCreateData, EventField>? originalConstructor)) 
                {
                    Type originalConstructorType = originalConstructor.GetType();

                    // 이미 존재하던 리더가 Johwa 라이브러리에서 등록된 경우
                    if (originalConstructorType.Assembly == currentAssembly)
                    {
                        // 리더 생성 시도
                        Func<EventFieldCreateData, EventField>? constructor;
                        if (TryCreateConstructor(type, out constructor) == false) 
                            continue; // 실패 시 종료

                        // 덮어쓰기
                        dictionary[fieldType] = constructor;
                        continue;
                    }
                    // 라이브러리 외부에서 등록한 경우
                    else 
                    {
                        JohwaLogger.Log($"중복 정의된 EventPropertyReader ({fieldType})가 있습니다. : {type.Name}, {originalConstructorType.Name}",
                            severity: LogSeverity.Warning, stackTrace: true);
                        continue;
                    }
                }
                // 대상 타입의 리더가 아직 없을 경우 
                else
                {
                    // 리더 생성 시도
                    Func<EventFieldCreateData, EventField>? constructor;
                    if (TryCreateConstructor(type, out constructor) == false) 
                        continue; // 실패 시 종료

                    // 값 추가
                    dictionary.Add(type, constructor);
                    continue;
                }
            }
        }
        return dictionary;

        /// <summary>
        /// 리플렉션으로 리더 타입의 인스턴스 생성 <br/>
        /// 타입이 제네릭임 -> $"EventPropertyReader에서 제네릭 클래스는 지원하지 않습니다. : {type}" <br/>
        /// 생성 실패 -> {type.Name} (EventPropertyReader) 생성에 실패하였습니다. : {type}
        /// </summary>
        /// <param name="readerType"></param>
        /// <param name="reader"></param>
        /// <returns>성공 여부</returns>
        static bool TryCreateConstructor(Type fieldType, [NotNullWhen(true)] out Func<EventFieldCreateData, EventField>? reader)
        {
            // 제네릭 클래스임 
            if (fieldType.IsGenericType) {
                JohwaLogger.Log($"EventPropertyReader에서 제네릭 클래스는 지원하지 않습니다. : {fieldType}",
                    severity: LogSeverity.Warning, stackTrace: true);

                reader = null;
                return false;
            }

            reader = GetConstructor(fieldType);
            if (reader == null) {
                JohwaLogger.Log($"{fieldType.Name} (EventPropertyReader) 생성에 실패하였습니다. : {fieldType}",
                    severity: LogSeverity.Warning, stackTrace: true);
                return false;
            }
            return true;
        }
    }
    static Func<EventFieldCreateData, EventField>? GetConstructor(Type fieldType)
    {
        // 캐시된 생성자 정보 로드
        if (constructorDictionary.TryGetValue(fieldType, out Func<EventFieldCreateData, EventField>? constructor)) {
            return constructor;
        }

        // 생성자 정보 생성
        ConstructorInfo? constructorInfo = fieldType.GetConstructor([ typeof(EventFieldCreateData) ]);
        if (constructorInfo == null) {
            return null;
        }

        // 파라미터
        ParameterExpression parameter = Expression.Parameter(typeof(EventFieldCreateData), "createData");

        // Expression
        NewExpression expression = Expression.New(constructorInfo, parameter);
        Expression<Func<EventFieldCreateData, EventField>> lambda = Expression.Lambda<Func<EventFieldCreateData, EventField>>(expression, parameter);
        
        // 생성자 컴파일
        constructor = lambda.Compile();

        // 생성자 캐싱
        constructorDictionary.Add(fieldType, constructor);
        
        // 반환
        return constructor;
    }
    
    #endregion

    #endregion


    #region Instance


    #endregion
}