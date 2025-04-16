using Johwa.Common.Debug;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace Johwa.Event.Data.PropertyReaders;

/// <summary>
/// EventPropertyReaderAttribute가 붙은 변수를 EventDataGroup에서 초기화할 때의 행동을 지정하는 클래스 <br/>
/// <br/>
/// 항상 EventPropertyReaderAttribute와 함께 사용되어야 합니다. <br/>
/// 대상 타입에 대해 단 한개만 리더가 존재해야 합니다. <br/>
/// 단, Johwa 라이브러리에서 정의된 리더는 덮어씌워질 수 있습니다.
/// </summary>
public abstract class EventPropertyReader
{
    #region Object

    [AttributeUsage(AttributeTargets.Class)]
    protected class EventPropertyReaderAttribute : Attribute
    {
        public Type fieldType;
        public EventPropertyReaderAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }

    #endregion


    #region Static

    public readonly static Dictionary<Type, EventPropertyReader> instanceDictionary = CreateInstanceDictionary();
    static Dictionary<Type, EventPropertyReader> CreateInstanceDictionary()
    {
        Dictionary<Type, EventPropertyReader> dictionary = new();
        
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            Type[] types = assembly.GetTypes();

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type readerType = types[typeIndex];
                
                // 클래스가 아니면 제외
                if (readerType.IsClass == false)
                    continue;
                
                // EventPropertyReader 상속 클래스가 아니면 제외
                if (readerType.IsSubclassOf(typeof(EventPropertyReader)) == false)
                    continue;

                // Attribute 로드
                EventPropertyReaderAttribute? attribute = readerType.GetCustomAttribute<EventPropertyReaderAttribute>();
                if (attribute == null) {
                    JohwaLogger.Log($"프로퍼티 리더({readerType})에 EventPropertyReaderAttribute가 없습니다.",
                        severity: LogSeverity.Warning, stackTrace: true);
                    continue;
                }
                Type propertyType = attribute.fieldType;
                
                // 대상 타입을 정의하는 리더가 이미 존재하면
                if (dictionary.TryGetValue(propertyType, out EventPropertyReader? originalReader)) 
                {
                    Type originalReaderType = originalReader.GetType();

                    // 이미 존재하던 리더가 Johwa 라이브러리에서 등록된 경우
                    if (originalReaderType.Assembly == currentAssembly)
                    {
                        // 리더 생성 시도
                        EventPropertyReader? reader;
                        if (TryCreateReader(readerType, out reader) == false) 
                            continue; // 실패 시 종료

                        // 덮어쓰기
                        dictionary[propertyType] = reader;
                        continue;
                    }
                    // 라이브러리 외부에서 등록한 경우
                    else 
                    {
                        JohwaLogger.Log($"중복 정의된 EventPropertyReader ({propertyType})가 있습니다. : {readerType.Name}, {originalReaderType.Name}",
                            severity: LogSeverity.Warning, stackTrace: true);
                        continue;
                    }
                }
                // 대상 타입의 리더가 아직 없을 경우 
                else
                {
                    // 리더 생성 시도
                    EventPropertyReader? reader;
                    if (TryCreateReader(readerType, out reader) == false)
                        continue; // 실패 시 종료

                    // 값 추가
                    dictionary.Add(readerType, reader);
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
        static bool TryCreateReader(Type readerType, [NotNullWhen(true)] out EventPropertyReader? reader)
        {
            // 제네릭 클래스임
            if (readerType.IsGenericType) {
                JohwaLogger.Log($"EventPropertyReader에서 제네릭 클래스는 지원하지 않습니다. : {readerType}",
                    severity: LogSeverity.Warning, stackTrace: true);

                reader = null;
                return false;
            }

            reader = (EventPropertyReader?)Activator.CreateInstance(readerType);
            if (reader == null) {
                JohwaLogger.Log($"{readerType.Name} (EventPropertyReader) 생성에 실패하였습니다. : {readerType}",
                    severity: LogSeverity.Warning, stackTrace: true);
                return false;
            }
            return true;
        }
    }

    public static EventPropertyReader? GetInstance(Type fieldType)
    {
        Type? findType = fieldType;
        while (findType != null)
        {
            EventPropertyReader? result;
            if (instanceDictionary.TryGetValue(findType, out result))
                return result;

            findType = findType.BaseType;
        }
        return null;
    }
    
    #endregion

    public abstract bool TryReadProperty(EventPropertyCreateData createData, [NotNullWhen(true)] out EventProperty? property);
}