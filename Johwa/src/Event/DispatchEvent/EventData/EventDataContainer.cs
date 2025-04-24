using System.Text.Json;

namespace Johwa.Event.Data;

public abstract class EventDataContainerMetadata
{
    #region Instance

    public readonly Type containerType;
    public readonly EventDataDescriptorTree dataDescriptorTree;
    public readonly EventFieldDescriptor[] fieldDescriptorArray;
    public readonly EventDataGroupDescriptor[] dataGroupDescriptorArray;
    public readonly int minDataCount;

    protected EventDataContainerMetadata(Type containerType)
    {
        this.containerType = containerType;

        EventDataDescriptor[] dataDescriptorArray;

        IEventDataGroup.CreateDescriptors(containerType, 
            out fieldDescriptorArray, out dataGroupDescriptorArray,
            out dataDescriptorArray, out minDataCount);
        
        dataDescriptorTree = new EventDataDescriptorTree(dataDescriptorArray);
    }

    #endregion
}

public abstract class EventDataContainer : IEventDataGroup, IDisposable
{
    #region Instance

    #region 재정의

    // 재정의 (IDisposable)
    public virtual void Dispose()
    {
        for (int i = 0; i < eventDataList.Count; i++)
        {
            eventDataList[i].Dispose();
        }
    }

    #endregion

    public abstract EventDataContainerMetadata ContainerMetadata { get; }
    public List<EventData> eventDataList;

    public ReadOnlyMemory<byte> data;

    #endregion

    public EventDataContainer(ReadOnlyMemory<byte> data)
    {
        this.data = data;

        CreateData(ContainerMetadata, data, 
            out eventDataList
        );
    }

    public static void CreateData(EventDataContainerMetadata metadata, ReadOnlyMemory<byte> data,
        out List<EventData> eventDataList)
    {
        eventDataList = new List<EventData>(metadata.minDataCount);

        // Json 읽기
        ReadOnlySpan<byte> dataSpan = data.Span;
        Utf8JsonReader jsonReader = new(dataSpan);

        while (jsonReader.Read())
        {
            // 이름만 필터링
            if (jsonReader.TokenType != JsonTokenType.PropertyName) continue;

            // 데이터 이름
            ReadOnlySpan<byte> jsonDataNameSpan = jsonReader.ValueSpan;

            EventDataDescriptor? dataDescriptor = metadata.dataDescriptorTree.GetDescriptor(jsonDataNameSpan);
            if (dataDescriptor == null) {
                continue;
            }

            EventData.EventDataCreateData dataCreateData = new (
                dataDescriptor, 
                GetDeclairingGroup(),
                data, 
                jsonReader.TokenType
            );

            EventData? eventData = dataDescriptor.CreateData(dataCreateData);
            if (eventData == null) {
                continue;
            }
            eventDataList.Add(eventData);
        }
    }
    static IEventDataGroup GetDeclairingGroup()
    {

    }
    
}