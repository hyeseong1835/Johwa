using Johwa.Event.Data;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

public class EventValueProperty : EventField
{
    static ConcurrentDictionary<Type, Func<EventFieldCreateData, EventValueProperty>> constructorDictionary = new();
    public static EventValueProperty CreateInstance(Type type, EventFieldCreateData createData)
    {
        // 생성자 로드 
        Func<EventFieldCreateData, EventValueProperty>? constructor;
        if (constructorDictionary.TryGetValue(type, out constructor) == false) 
        {
            // 생성자 로드
            ConstructorInfo constructorInfo = propertyType.GetConstructor([ typeof(EventFieldCreateData) ])!;

            // 파라미터
            ParameterExpression parameter = Expression.Parameter(typeof(EventFieldCreateData), "createData");

            // Expression
            NewExpression expression = Expression.New(constructorInfo, parameter);

            // Lambda
            var lambda = Expression.Lambda<Func<EventFieldCreateData, EventValueProperty>>(expression);
            
            // 생성자
            constructor = lambda.Compile();

            constructorDictionary.TryAdd(type, constructor);
        }

        return constructor.Invoke(createData);
    }
}