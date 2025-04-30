using System.Diagnostics.CodeAnalysis;

namespace Johwa.Common.Extension.System;

public static class TypeExtension
{
    /// <summary>
    /// 대상 타입이 지정한 제네릭 타입 정의의 구체화된 타입을 상속하는 지 확인하고 구체화된 타입을 반환합니다. <br/>
    /// </summary>
    /// <param name="type">대상 타입</param>
    /// <param name="targetGenericTypeDefinition">찾을 제네릭 타입의 정의 (제네릭 타입 정의 부분이 비어있는 열린 제네릭 타입)</param>
    /// <param name="findGenericType">대상 타입이 상속하는 지정한 제네릭 타입 정의의 구체화된 타입</param>
    /// <returns>
    /// true: "targetGenericTypeDefinition"의 구체화된 타입을 상속함 <br/>
    /// false: "targetGenericTypeDefinition"의 구체화된 타입을 상속하지 않음
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsSubclassOfGenericClass(this Type type, Type targetGenericTypeDefinition, [NotNullWhen(true)] out Type? findGenericType)
    {
        if (targetGenericTypeDefinition.IsGenericTypeDefinition == false) 
            throw new ArgumentException(
                $"{nameof(targetGenericTypeDefinition)}는 열린 제네릭 타입이어야 합니다.", 
                nameof(targetGenericTypeDefinition)
            );
        
        Type? findType = type;

        while (findType != null)
        {
            if (findType.IsGenericType 
                && findType.GetGenericTypeDefinition() == targetGenericTypeDefinition)
            {
                findGenericType = findType;
                return true;
            }

            // 계속 부모 타입으로 올라간다.
            findType = findType.BaseType;
            continue;
        }

        findGenericType = null;
        return false;
    }
}