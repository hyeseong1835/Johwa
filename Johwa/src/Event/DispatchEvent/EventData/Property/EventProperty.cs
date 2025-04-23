using System.Linq.Expressions;
using System.Reflection;

namespace Johwa.Event.Data;

public abstract class EventProperty : EventData
{
    #region Static

    static Dictionary<Type, Func<EventDataCreateData, EventProperty>> constructorDictionary = new();
    public static EventProperty CreateInstance(EventDataCreateData createData)
    {
        // 생성자 로드 
        Func<EventDataCreateData, EventProperty>? constructor;
        if (constructorDictionary.TryGetValue(createData., out constructor) == false) 
        {
            // 생성자 로드
            ConstructorInfo constructorInfo = propertyGenericType.GetConstructor([ typeof(EventDataCreateData) ])!;

            // 파라미터
            ParameterExpression parameter = Expression.Parameter(typeof(EventDataCreateData), "createData");

            // Expression
            NewExpression expression = Expression.New(constructorInfo, parameter);

            // Lambda
            var lambda = Expression.Lambda<Func<EventDataCreateData, EventProperty>>(expression);
            
            // 생성자
            constructor = lambda.Compile();

            constructorDictionary.Add(propertyGenericType, constructor);
        }

        return constructor.Invoke(createData);
    }
    public static EventProperty CreateInstance(EventDataCreateData createData)
    {
        // EventPropertyDescriptor를 EventProperty로 변환
        EventPropertyDescriptor? descriptor = createData.descriptor as EventPropertyDescriptor;
        if (descriptor == null)
            throw new Exception($"EventPropertyDescriptor가 아닙니다. : {createData.descriptor}");

        
        
    }

    #endregion


    #region Instance

    // 필드
    public EventPropertyDescriptor descriptor;

    // 생성자
    public EventProperty(EventDataCreateData dataCreateData)
    {
        this.descriptor = (EventPropertyDescriptor)dataCreateData.descriptor;
    }


    #endregion
}
public class EventProperty<T> : EventProperty
{
    #region Instance

    public T? value;
    public bool isValueReaded;

    // 생성자
    public EventProperty(EventDataCreateData createData) 
        : base(createData) 
    { 

    }

    #endregion
}