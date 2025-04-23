namespace Johwa.Event.Data;

public class DeferredReadField : EventField
{
    #region Instance

    public DeferredReadField(EventFieldDescriptor descriptor, IEventDataGroup declaringGroup,
        ReadOnlyMemory<byte> data, JsonTokenType tokenType) : base(descriptor, declaringGroup, data, tokenType)
    {
    }

    #endregion
}