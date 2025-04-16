using Johwa.Event.Data;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;


public abstract class EventValueProperty : EventProperty
{
    static ConcurrentDictionary<Type, Func<EventPropertyCreateData, EventValueProperty>> constructorDictionary = new();
    public static EventValueProperty CreateInstance(Type type, EventPropertyCreateData createData)
    {
        // 생성자 로드 
        Func<EventPropertyCreateData, EventValueProperty>? constructor;
        if (constructorDictionary.TryGetValue(type, out constructor) == false) 
        {
            // 제네릭 EventValueProperty 타입 생성
            Type propertyType = typeof(EventValueProperty<>).MakeGenericType(type);

            // 생성자 로드
            ConstructorInfo constructorInfo = propertyType.GetConstructor([ typeof(EventPropertyCreateData) ])!;

            // 파라미터
            ParameterExpression parameter = Expression.Parameter(typeof(EventPropertyCreateData), "createData");

            // Expression
            NewExpression expression = Expression.New(constructorInfo, parameter);

            // Lambda
            var lambda = Expression.Lambda<Func<EventPropertyCreateData, EventValueProperty>>(expression);
            
            // 생성자
            constructor = lambda.Compile();

            constructorDictionary.TryAdd(type, constructor);
        }

        return constructor.Invoke(createData);
    }
}
public class EventValueProperty<T> : EventValueProperty
{
    public EventValueProperty(EventPropertyCreateData createData)
    {
        
    }
}