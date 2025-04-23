using Johwa.Event.Data;
using System.Linq.Expressions;
using System.Reflection;

public abstract class EventValueProperty : EventProperty
{
    static Dictionary<Type, Func<EventDataCreateData, EventValueProperty>> constructorDictionary = new();
    public static EventValueProperty CreateInstance(EventDataCreateData createData)
    {
        // 생성자 로드 
        Func<EventDataCreateData, EventValueProperty>? constructor;

        Type genericValuePropertyType = createData.
        if (constructorDictionary.TryGetValue(type, out constructor) == false) 
        {
            // 생성자 로드
            ConstructorInfo constructorInfo = propertyType.GetConstructor([ typeof(EventDataCreateData) ])!;

            // 파라미터
            ParameterExpression parameter = Expression.Parameter(typeof(EventDataCreateData), "createData");

            // Expression
            NewExpression expression = Expression.New(constructorInfo, parameter);

            // Lambda
            var lambda = Expression.Lambda<Func<EventDataCreateData, EventValueProperty>>(expression);
            
            // 생성자
            constructor = lambda.Compile();

            constructorDictionary.TryAdd(type, constructor);
        }

        return constructor.Invoke(createData);
    }

    public EventValueProperty(EventDataCreateData dataCreateData) : base(dataCreateData)
    {
        // 생성자
    }
}
public class EventValueProperty<TValue> : EventValueProperty
{
    public TValue? value;

    public EventValueProperty(EventDataCreateData dataCreateData) : base(dataCreateData)
    {
        // 생성자
    }
}