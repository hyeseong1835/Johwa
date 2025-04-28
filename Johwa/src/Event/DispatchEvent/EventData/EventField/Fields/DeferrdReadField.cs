namespace Johwa.Event.Data;

public class DeferredReadField : EventField
{
    #region Instance

    public DeferredReadField(EventFieldInfo descriptor, IEventDataGroup declaringGroup,
        ReadOnlyMemory<byte> data, JsonTokenType tokenType) : base(descriptor, declaringGroup, data, tokenType)
    {
    }

    #endregion
}