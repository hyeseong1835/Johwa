using System.Buffers;
using System.Text.Json;
using Johwa.Common.Collection;
using Johwa.Common.Extension.System.Text.Json;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    public Type eventDataType;
    public readonly EventPropertyGroupDescriptorAttribute[] propertyGroupDescriptorArray;
    public readonly EventPropertyDescriptorAttribute[] propertyDescriptorArray;

    public EventDataDocumentMetadata(Type eventDataType)
    {
        this.eventDataType = eventDataType;
        propertyGroupDescriptorArray = IEventDataContainer.LoadPropertyGroupDescriptors(eventDataType).ToArray();
        propertyDescriptorArray = IEventDataContainer.LoadPropertyDataDescriptors(eventDataType).ToArray();
    }
}

public abstract class EventDataDocument : IEventDataContainer
{
    #region 재정의

    IEnumerable<EventPropertyData> IEventDataContainer.GetPropertyDataEnumerable()
    {
        for (int i = 0; i < propertyData.Count; i++)
        {
            EventPropertyData property = propertyData[i];
            yield return property;
        }
    }
    void IDisposable.Dispose()
    {
        data = ReadOnlyMemory<byte>.Empty;

        foreach (IDisposable property in propertyData)
        {
            property.Dispose();
        }
    }
    
    #endregion


    #region 필드

    public readonly EventDataDocumentMetadata metadata;
    public ReadOnlyMemory<byte> data;
    public readonly List<EventPropertyData> propertyData;

    #endregion


    #region 생성자

    public EventDataDocument(ReadOnlyMemory<byte> data)
    {
        this.data = data;
        this.metadata = GetMetadata();
        this.propertyData = CreateProperties();
    }

    #endregion
    

    #region 추상
    
    protected abstract EventDataDocumentMetadata GetMetadata();

    #endregion


    #region 메서드

    

    #endregion
}