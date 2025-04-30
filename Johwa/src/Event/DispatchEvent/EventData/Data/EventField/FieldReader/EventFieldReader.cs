using System.Reflection;
using System.Text.Json;
using Johwa.Common.Debug;
using Johwa.Common.Extension.System;

namespace Johwa.Event.Data;

public abstract class EventFieldReader
{
    #region Object

    struct FieldReaderData
    {
        public Assembly assembly;
        public Type readerType;
        public Type targetFieldType;

        public FieldReaderData(Assembly assembly, Type readerType, Type targetFieldType)
        {
            this.assembly = assembly;
            this.readerType = readerType;
            this.targetFieldType = targetFieldType;
        }
    }
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
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 필드 리더 정보 로드
        Dictionary<Type, FieldReaderData> instanceTypeDataDictionary = CreateInstanceTypeDataDictionary(executingAssembly, assemblies);

        Type eventFieldReaderType = typeof(EventFieldReader);


        Dictionary<Type, EventFieldReader> instanceDictionary = new Dictionary<Type, EventFieldReader>();

        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];

            // 라이브러리 어셈블리는 오버라이드 될 수 있으므로 나중에 로드합니다.
            if (assembly == executingAssembly) 
                continue;

            ReadAssembly(assembly, eventFieldReaderType, instanceDictionary);
        }

        // 라이브러리 어셈블리는 오버라이드 될 수 있으므로 나중에 로드합니다.
        ReadAssembly(executingAssembly, eventFieldReaderType, instanceDictionary);

        return instanceDictionary;


        static void ReadAssembly(Assembly assembly, Type eventFieldReaderType, Dictionary<Type, EventFieldReader> instanceDictionary)
        {
            Type[] types = assembly.GetTypes();
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type readerType = types[typeIndex];

                // 추상 타입이면 패스
                if (readerType.IsAbstract)
                    continue;

                // 클래스가 아니면 패스
                if (readerType.IsClass == false)
                    continue;

                // EventFieldReader를 상속받지 않으면 패스
                if (readerType.IsSubclassOf(eventFieldReaderType) == false)
                    continue;

                // 인스턴스 생성
                EventFieldReader? instance = (EventFieldReader?)Activator.CreateInstance(readerType);
                if (instance == null) {
                    throw new JohwaException($"EventFieldReader ({readerType})를 생성할 수 없습니다.",
                        severity: LogSeverity.Error, stackTrace: true);
                }

                // [ A ] : B : C
                // C => C,B,A
                // B => B,A

                // A => A

                Type? fieldType = instance.FieldType;

                // 이미 필드타입에 대한 정의가 존재함 => 
                if (instanceDictionary.ContainsKey(fieldType))
                {
                    fieldType = fieldType.BaseType;
                    continue;
                }
                else
                {

                }

                while (fieldType != null)
                {

                    if (instanceDictionary.TryAdd(fieldType, instance))
                    {
                        fieldType = fieldType.BaseType;
                        continue;
                    }

                    // 값이 이미 존재함 => 현재 
                    break;
                }
            }
        }
    }
    static Dictionary<Type, FieldReaderData> CreateInstanceTypeDataDictionary(Assembly executingAssembly, Assembly[] assemblies)
    {
        Dictionary<Type, FieldReaderData> instanceTypeDataDictionary = new Dictionary<Type, FieldReaderData>();

        Type eventFieldReaderType = typeof(EventFieldReader);
        Type eventFieldReaderOpenGenericType = typeof(EventFieldReader<>);

        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];

            // 라이브러리 어셈블리는 오버라이드 될 수 있으므로 나중에 로드합니다.
            if (assembly == executingAssembly) 
                continue;

            Type[] types = assembly.GetTypes();

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type readerType = types[typeIndex];

                // 추상 타입이면 패스
                if (readerType.IsAbstract)
                    continue;

                // 클래스가 아니면 패스
                if (readerType.IsClass == false)
                    continue;

                if (readerType.IsSubclassOfGenericClass(eventFieldReaderOpenGenericType, out Type? findGenericType))
                {
                    Type[] genericTypeArguments = findGenericType.GetGenericArguments();
                    Type targetFieldType = genericTypeArguments[0];
                    FieldReaderData data = new FieldReaderData(assembly, readerType, targetFieldType);
                }

                FieldReaderData instanceTypeData = new FieldReaderData(assembly, readerType, eventFieldReaderType);

            instanceTypeDataDictionary.Add(readerType, instanceTypeData);

                FieldInfo[] fieldInfos = readerType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int fieldInfoIndex = 0; fieldInfoIndex < fieldInfos.Length; fieldInfoIndex++)
                {
                    FieldInfo fieldInfo = fieldInfos[fieldInfoIndex];
                    Type? fieldType = fieldInfo.FieldType;
                    FieldReaderData data = new FieldReaderData(assembly, readerType, fieldType);

                    while (fieldType != null)
                    {
                        if (instanceTypeDataDictionary.TryAdd(fieldType, data))
                        {
                            fieldType = fieldType.BaseType;
                            continue;
                        }

                        // 값이 이미 존재함 => 현재 
                        break;
                    }
                }
            }

            
        }

        // 라이브러리 어셈블리는 오버라이드 될 수 있으므로 나중에 로드합니다.
        ReadAssembly(executingAssembly, eventFieldReaderType, instanceTypeDataDictionary);

        return instanceTypeDataDictionary;


        static void ReadAssembly(Assembly assembly, Type eventFieldReaderType, Dictionary<Type, FieldReaderData> instanceTypeDataDictionary)
        {
            instanceDictionary.TryGetValue(fieldType, out EventFieldReader? instance);
            Type[] types = assembly.GetTypes();
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type readerType = types[typeIndex];

                // 추상 타입이면 패스
                if (readerType.IsAbstract)
                    continue;

                // 클래스가 아니면 패스
                if (readerType.IsClass == false)
                    continue;

                // EventFieldReader를 상속받지 않으면 패스
                if (readerType.IsSubclassOf(eventFieldReaderType) == false)
                    continue;

                FieldInfo[] fieldInfos = readerType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                for (int fieldInfoIndex = 0; fieldInfoIndex < fieldInfos.Length; fieldInfoIndex++)
                {
                    FieldInfo fieldInfo = fieldInfos[fieldInfoIndex];
                    Type? fieldType = fieldInfo.FieldType;
                    FieldReaderData data = new FieldReaderData(readerType, fieldType);

                    while (fieldType != null)
                    {
                        if (instanceTypeDataDictionary.TryAdd(fieldType, data))
                        {
                            fieldType = fieldType.BaseType;
                            continue;
                        }

                        // 값이 이미 존재함 => 현재 
                        break;
                    }
                }
            }
        }
    }
    internal static void ReadField(Type fieldType, )
    {
        EventFieldReader instance = GetInstance(fieldType);
        
        IEventField.CreateData createData = new (
            createData.info, 
            createData.fieldPtr, 
            createData.data, 
            createData.tokenType
        );
        instance.Read(createData);
    }

    /// <summary>
    /// 생성된 리더를 찾고 없으면 새로 생성합니다.<br/>
    /// </summary>
    /// <param name="fieldType"></param>
    /// <returns></returns>
    /// <exception cref="JohwaException">EventFieldReader ({type})를 생성할 수 없습니다.</exception>
    static EventFieldReader GetInstance(Type fieldType)
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

    public abstract Type FieldType { get; }
    protected abstract void Read(IEventField.CreateData createData);

    #endregion
}

public abstract class EventFieldReader<TField> : EventFieldReader
    where TField : unmanaged
{
    public sealed override Type FieldType => typeof(TField);

    unsafe protected sealed override void Read(IEventField.CreateData createData)
    {
        TField* fieldPtr = (TField*)createData.fieldPtr;
        ReadData readData = new ReadData(createData);

        *fieldPtr = Read(readData);
    }
    protected abstract TField Read(ReadData data);
}