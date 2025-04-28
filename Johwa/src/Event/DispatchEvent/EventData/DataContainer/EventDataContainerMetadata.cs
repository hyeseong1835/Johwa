using Johwa.Common.Extension.System;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public abstract class EventDataContainerMetadata
{
    #region Instance

    public readonly Type containerType;
    public readonly ReadOnlyByteSpanTree<EventDataInfo> dataDescriptorTree;
    public readonly EventDataGroupInfo[] dataGroupDescriptorArray;
    public readonly EventFieldInfo[] fieldDescriptorArray;
    public readonly int minDataCount;

    protected EventDataContainerMetadata(Type containerType)
    {
        this.containerType = containerType;

        EventDataInfo[] dataDescriptorArray;

        IEventDataGroup.CreateDescriptors(
            containerType, 
            out dataGroupDescriptorArray, 
            out fieldDescriptorArray, 
            out dataDescriptorArray, 
            out minDataCount
        );

        // 설명자 트리
        ReadOnlyByteSpanTree<EventDataInfo>.Builder dataDescriptorTreeBuilder = new();
        for (int i = 0; i < dataDescriptorArray.Length; i++)
        {
            EventDataInfo dataDescriptor = dataDescriptorArray[i];
            dataDescriptorTreeBuilder.Add(dataDescriptor.name.AsByteSpan(), dataDescriptor);
        }
        dataDescriptorTree = dataDescriptorTreeBuilder.BuildAndDispose();
    }

    #endregion
}
