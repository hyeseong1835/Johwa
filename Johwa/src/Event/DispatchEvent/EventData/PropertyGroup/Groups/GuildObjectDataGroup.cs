using Johwa.Common.Debug;
using Johwa.Event.Data;

public class GuildObjectDataGroup : EventPropertyGroupData
{
    #region Static

    // 필드
    public static EventPropertyGroupMetadata? metadata;

    #endregion


    #region Instance

    // 재정의
    public override EventPropertyGroupMetadata Metadata => metadata ??= EventPropertyGroupMetadata.GetInstance(typeof(GuildObjectDataGroup));

    // 필드

    // 생성자
    public GuildObjectDataGroup(
        EventPropertyGroupAttribute descriptor) : base(descriptor) { }

    #endregion
}