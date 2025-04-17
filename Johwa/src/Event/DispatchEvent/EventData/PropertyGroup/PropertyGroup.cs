namespace Johwa.Event.Data;

public class EventPropertyGroupMetadata
{
    #region Static

    public static Dictionary<Type, EventPropertyGroupMetadata> instanceDictionary = new Dictionary<Type, EventPropertyGroupMetadata>();
    public static EventPropertyGroupMetadata GetInstance(Type dataType)
    {
        if (instanceDictionary.TryGetValue(dataType, out EventPropertyGroupMetadata? result) == false)
        {
            result = new EventPropertyGroupMetadata(dataType);
            instanceDictionary[dataType] = result;
        }
        return result;
    }

    #endregion


    #region Instance

    // 필드
    public readonly Type dataType;
    public readonly EventFieldDescriptor[] propertyDescriptorArray;
    public readonly EventDataGroupAttribute[] propertyGroupAttributeArray;
    public IEnumerable<EventFieldDescriptor>[] 

    // 생성자
    public EventPropertyGroupMetadata(Type dataType)
    {
        this.dataType = dataType;
        this.propertyDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(dataType);
    }

    #endregion
}
public class EventDataGroupAttribute : Attribute
{
    // 필드
    public EventPropertyGroupMetadata? metadata;

    // 생성자
    public EventDataGroupAttribute() { }
}
public abstract class EventPropertyGroupData
{
    // 필드
    public abstract EventPropertyGroupMetadata Metadata { get; }
    public EventDataGroupAttribute descriptor;

    public EventPropertyGroupData(EventDataGroupAttribute descriptor)
    {
        this.descriptor = descriptor;
    }
}
