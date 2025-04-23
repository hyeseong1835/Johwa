using Johwa.Event.Data;

public class GuildObjectDataGroup : EventDataGroup
{
    #region Static

    // 필드
    new public static EventDataGroupMetadata metadata = new EventDataGroupMetadata(typeof(GuildObjectDataGroup));

    #endregion


    #region Instance

    // 필드

    // 생성자
    public GuildObjectDataGroup(CreateData createData) 
        : base(metadata, createData) { }

    #endregion
}