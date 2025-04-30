using System.Reflection;

namespace Johwa.Common.Utility.System;

public static class TypeUtility
{
    /// <summary>
    /// 로드된 모든 어셈블리의 타입을 열거합니다. <br/>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Type> GetAllTypes()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            Type[] types = assembly.GetTypes();

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type type = types[typeIndex];

                yield return type;
            }
        }
    }
}