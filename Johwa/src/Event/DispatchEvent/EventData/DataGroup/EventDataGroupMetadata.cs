namespace Johwa.Event.Data;

public class EventDataGroupMetadata
{
    #region Static

    public static Dictionary<Type, EventDataGroupMetadata> instanceDictionary = new Dictionary<Type, EventDataGroupMetadata>();
    public static EventDataGroupMetadata GetInstance(Type dataType)
    {
        if (instanceDictionary.TryGetValue(dataType, out EventDataGroupMetadata? result) == false)
        {
            result = new EventDataGroupMetadata(dataType);
            instanceDictionary[dataType] = result;
        }
        return result;
    }

    #endregion


    #region Instance

    // 필드
    public readonly Type dataType;
    public readonly EventFieldDescriptor[] subFieldDescriptorArray;
    public readonly EventPropertyDescriptor[] subPropertyDescriptorArray;
    public readonly EventDataGroupDescriptor[] subDataGroupDescriptorArray;

    // 생성자
    public EventDataGroupMetadata(Type dataType)
    {
        this.dataType = dataType;
        this.subFieldDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(dataType);
    }

    #endregion
}