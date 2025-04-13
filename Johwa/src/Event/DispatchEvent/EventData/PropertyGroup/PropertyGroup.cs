namespace Johwa.Event.Data;

public class EventPropertyGroupMetadata
{
    #region 재정의

    public Type ContainerType => dataType;

    #endregion

    #region Instance

    // 필드
    public readonly Type dataType;

    // 생성자
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
