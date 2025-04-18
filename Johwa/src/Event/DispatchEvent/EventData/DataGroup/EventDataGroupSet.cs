namespace Johwa.Event.Data;

public class EventDataGroupSet
{
    // 필드
    readonly EventDataGroup[] array;

    public EventDataGroupSet(EventDataGroupDescriptor[] descriptors)
    {
        array = new EventDataGroup[descriptors.Length];

        // 그룹 배열 채우기
        for (int i = 0; i < descriptors.Length; i++)
        {
            EventDataGroupDescriptor descriptor = descriptors[i];

            // 그룹 생성
            EventDataGroup.CreateData createData = new EventDataGroup.CreateData(descriptor);
            array[i] = descriptor.CreateGroup(createData);
        }
    }
}