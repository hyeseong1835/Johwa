using Johwa.Event.Data;

public class GuildObjectDataGroup : EventDataGroupData
{
    #region Static

    // 필드
    public static EventDataGroupMetadata? metadata;

    #endregion


    #region Instance

    // 재정의
    public override EventDataGroupMetadata Metadata 
        => metadata ??= EventDataGroupMetadata.GetInstance(typeof(GuildObjectDataGroup));

    // 필드

    // 생성자
    public GuildObjectDataGroup(
        EventDataGroupAttribute descriptor) : base(descriptor) { }

    #endregion
}