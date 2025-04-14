namespace Johwa.Event.Data;

public class EventPropertyGroupMetadata : IEventDataGroupMetadata
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

    #region 재정의

    Type IEventDataGroupMetadata.GroupType => dataType;
    EventPropertyDescriptorAttribute[] IEventDataGroupMetadata.PropertyDescriptorArray => propertyDescriptorArray;

    #endregion

    #region Instance

    // 필드
    public readonly Type dataType;
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    // 생성자
    public EventPropertyGroupMetadata(Type dataType)
    {
        this.dataType = dataType;
        this.propertyDescriptorArray = IEventDataGroupMetadata.LoadPropertyDescriptors(dataType);
    }

    #endregion
}
public class EventPropertyGroupAttribute : Attribute
{
    // 필드
    public EventPropertyGroupMetadata? metadata;

    // 생성자
    public EventPropertyGroupAttribute() { }
}
public abstract class EventPropertyGroupData
{
    // 필드
    public abstract EventPropertyGroupMetadata Metadata { get; }
    public EventPropertyGroupAttribute descriptor;

    public EventPropertyGroupData(EventPropertyGroupAttribute descriptor)
    {
        this.descriptor = descriptor;
    }
}
