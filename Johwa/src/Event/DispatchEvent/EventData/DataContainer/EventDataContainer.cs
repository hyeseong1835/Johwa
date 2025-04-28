using System.Text.Json;
using Johwa.Common.Debug;
using System.Text;

namespace Johwa.Event.Data;

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

            EventDataDescriptor? dataDescriptor;
            if (metadata.dataDescriptorTree.TryGetValue(jsonDataNameSpan, out dataDescriptor) == false)
            {
                // 데이터 설명자를 찾지 못함
                JohwaLogger.Log($"'{Encoding.ASCII.GetString(jsonDataNameSpan)}' 키를 찾을 수 없습니다.");
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