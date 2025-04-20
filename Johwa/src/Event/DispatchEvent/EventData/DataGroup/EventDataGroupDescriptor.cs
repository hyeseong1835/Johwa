using System.Reflection;

namespace Johwa.Event.Data;

public class EventDataGroupDescriptor
{
    public EventDataGroupMetadata metadata;
    public FieldInfo fieldInfo;

    public EventDataGroupDescriptor(FieldInfo fieldInfo, EventDataGroupMetadata metadata)
    {
        this.fieldInfo = fieldInfo;
        this.metadata = metadata;
    }

    public IEnumerable<EventDataDescriptor> GetEventDataDescriptorEnumerable()
    {
        // 필드
        for (int i = 0; i < metadata.subFieldDescriptorArray.Length; i++)
        {
            yield return metadata.subFieldDescriptorArray[i];
        }

        // 프로퍼티
        for (int i = 0; i < metadata.subPropertyDescriptorArray.Length; i++)
        {
            yield return metadata.subPropertyDescriptorArray[i];
        }

        // 그룹
        for (int i = 0; i < metadata.subDataGroupDescriptorArray.Length; i++)
        {
            EventDataGroupDescriptor descriptor = metadata.subDataGroupDescriptorArray[i];
            foreach (EventDataDescriptor dataDescriptor in descriptor.GetEventDataDescriptorEnumerable())
            {
                yield return dataDescriptor;
            }
        }
    }
}