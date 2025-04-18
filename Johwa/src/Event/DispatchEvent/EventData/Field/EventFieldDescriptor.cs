using System.Reflection;
using Johwa.Common.Debug;

namespace Johwa.Event.Data;

public class EventFieldDescriptor
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

        public static bool IsNameMatch(EventFieldDescriptor descriptor, NameId id)
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

    public static bool IsNameMatch(EventFieldDescriptor descriptor, ReadOnlyMemory<byte> nameMemory)
    {
        if (descriptor.name.Length != nameMemory.Length)
            return false;

        ReadOnlySpan<byte> nameSpan = nameMemory.Span;
        for (int i = 0; i < descriptor.name.Length; i++)
        {
            if (nameSpan[i] != descriptor.name[i])
                return false;
        }

        return true;
    }

    #endregion


    #region Instance

    public readonly FieldInfo fieldInfo;
    public readonly bool isFieldTypeEventProperty;

    public readonly string name;
    public readonly bool isOptional;
    public readonly bool isNullable;

    public EventFieldDescriptor(FieldInfo fieldInfo, bool isFieldTypeEventProperty, string name, bool isOptional, bool isNullable)
    {
        this.fieldInfo = fieldInfo;
        this.isFieldTypeEventProperty = isFieldTypeEventProperty;
        this.name = name;
        this.isOptional = isOptional;
        this.isNullable = isNullable;
    }

    #endregion
}