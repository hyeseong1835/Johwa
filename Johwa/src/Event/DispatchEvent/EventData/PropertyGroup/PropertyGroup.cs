namespace Johwa.Event.Data;

public class EventPropertyGroupMetadata
{
    #region Instance

    public readonly Type dataType;

    public EventPropertyGroupMetadata(Type dataType)
    {
        this.dataType = dataType;
    }

    #endregion
}
public class EventPropertyGroupDescriptorAttribute : Attribute
{
    // 필드
    public readonly EventPropertyGroupMetadata? metadata;

    // 생성자
    public EventPropertyGroupDescriptorAttribute() { }
}
public abstract class EventPropertyGroupData
{
    // 필드
    public abstract EventPropertyGroupDescriptorAttribute Descriptor { get; }
}
