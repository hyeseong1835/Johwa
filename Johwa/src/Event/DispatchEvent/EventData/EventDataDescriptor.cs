using System.Reflection;

namespace Johwa.Event.Data;

public abstract class EventDataDescriptor
{
    #region Object

    unsafe public struct NameId
    {
        public readonly int length;
        public fixed byte name[64];

        public NameId(ReadOnlySpan<byte> nameSpan)
        {
            length = nameSpan.Length;
            if (length > 64)
                throw new ArgumentOutOfRangeException(nameof(nameSpan), "이름의 길이는 64를 초과할 수 없습니다.");

            for (int i = 0; i < length; i++)
                name[i] = nameSpan[i];
        }

        public static bool IsNameMatch(EventPropertyDescriptor descriptor, NameId id)
        {
            if (descriptor.name.Length != id.length)
                return false;

            for (int i = 0; i < descriptor.name.Length; i++)
            {
                if (descriptor.name[i] != id.name[i])
                    return false;
            }

            return true;
        }
    }

    #endregion


    #region Static

    static Dictionary<Type, EventDataDescriptor> instanceDictionary = new ();

    public static EventDataDescriptor GetInstance(FieldInfo fieldInfo)
    {
        if (instanceDictionary.TryGetValue(groupType, out EventDataDescriptor? descriptor) == false)
        {
            descriptor = (EventDataDescriptor)Activator.CreateInstance(groupType)!;
            instanceDictionary.Add(groupType, descriptor);
        }
    }

    #endregion
    

    #region Instance

    public abstract string Name { get; }

    #endregion
}