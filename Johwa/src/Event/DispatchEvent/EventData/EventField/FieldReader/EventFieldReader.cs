using System.Reflection;
using System.Text.Json;
using Johwa.Common.Debug;

namespace Johwa.Event.Data;

public abstract class EventFieldReader
{
    #region Object

    protected struct ReadData
    {
        public EventFieldInfo info;
        public ReadOnlyMemory<byte> data;
        public JsonTokenType jsonTokenType;

        public ReadData(IEventField.CreateData createData) 
            : this(createData.info, createData.data, createData.tokenType) { }
        public ReadData(EventFieldInfo info, 
            ReadOnlyMemory<byte> data, JsonTokenType tokenType)
        {
            this.info = info;
            this.data = data;
            this.jsonTokenType = tokenType;
        }
    }

    #endregion

    #region Static

    // 필드
    static Dictionary<Type, EventFieldReader> instanceDictionary = CreateInstanceDictionary();

    #region 메서드

    static Dictionary<Type, EventFieldReader> CreateInstanceDictionary()
    {
        Dictionary<Type, EventFieldReader> instanceDictionary = new Dictionary<Type, EventFieldReader>();

        Type eventFieldReaderType = typeof(EventFieldReader);

        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];

            // 라이브러리 어셈블리는 오버라이드 될 수 있으므로 나중에 로드합니다.
            if (assembly == executingAssembly) 
                continue;

            Type[] types = assembly.GetTypes();
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type type = types[typeIndex];

                // 추상 타입이면 패스
                if (type.IsAbstract)
                    continue;

                // 클래스가 아니면 패스
                if (type.IsClass == false)
                    continue;

                if (type.IsSubclassOf(eventFieldReaderType))
                {
                    EventFieldReader instance = (EventFieldReader)Activator.CreateInstance(type)!;
                    instanceDictionary.Add(type.BaseType!, instance);
                }
            }
        }

        return instanceDictionary;


        static void ReadAssembly(Assembly assembly, )
        {

        }
    }
    internal static void ReadField(IEventField.CreateData createData)
    {
        EventFieldReader instance = GetOrCreateInstance(createData.info.fieldInfo.FieldType);
        instance.Read(createData);
    }

    /// <summary>
    /// 생성된 리더를 찾고 없으면 새로 생성합니다.<br/>
    /// </summary>
    /// <param name="fieldType"></param>
    /// <returns></returns>
    /// <exception cref="JohwaException">EventFieldReader ({type})를 생성할 수 없습니다.</exception>
    static EventFieldReader GetOrCreateInstance(Type fieldType)
    {
        if (instanceDictionary.TryGetValue(fieldType, out EventFieldReader? instance))
        {
            return instance;
        }
        else
        {
            instance = (EventFieldReader?)Activator.CreateInstance(fieldType);
            if (instance == null)
            {
                throw new JohwaException($"EventFieldReader ({fieldType})를 생성할 수 없습니다.",
                    severity: LogSeverity.Error, stackTrace: true);
            }
            else
            {
                return instance;
            }
        }
    }
    
    #endregion
    
    #endregion

    #region Instance

    protected abstract void Read(IEventField.CreateData createData);

    #endregion
}

public abstract class EventFieldReader<TField> : EventFieldReader
    where TField : unmanaged
{
    unsafe protected sealed override void Read(IEventField.CreateData createData)
    {
        TField* fieldPtr = (TField*)createData.fieldPtr;
        ReadData readData = new ReadData(createData);

        *fieldPtr = Read(readData);
    }
    protected abstract TField Read(ReadData data);
}