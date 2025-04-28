using Johwa.Common.Extension.System;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public abstract class EventDataContainerMetadata
{
    #region Instance

    public readonly Type containerType;
    public readonly ReadOnlyByteSpanTree<EventDataDescriptor> dataDescriptorTree;
    public readonly EventDataGroupDescriptor[] dataGroupDescriptorArray;
    public readonly EventFieldDescriptor[] fieldDescriptorArray;
    public readonly int minDataCount;

    protected EventDataContainerMetadata(Type containerType)
    {
        this.containerType = containerType;

        EventDataDescriptor[] dataDescriptorArray;

        IEventDataGroup.CreateDescriptors(
            containerType, 
            out dataGroupDescriptorArray, 
            out fieldDescriptorArray, 
            out dataDescriptorArray, 
            out minDataCount
        );

        // 설명자 트리
        ReadOnlyByteSpanTree<EventDataDescriptor>.Builder dataDescriptorTreeBuilder = new();
        for (int i = 0; i < dataDescriptorArray.Length; i++)
        {
            EventDataDescriptor dataDescriptor = dataDescriptorArray[i];
            dataDescriptorTreeBuilder.Add(dataDescriptor.name.AsByteSpan(), dataDescriptor);
        }
        dataDescriptorTree = dataDescriptorTreeBuilder.BuildAndDispose();
    }

    #endregion
}
