namespace Johwa.Event.Data;


public abstract class EventObjectPropertyData : EventProperty
{
    #region Object

    new public abstract class Metadata : EventProperty.Metadata, IEventDataContainerMetadata
    {
        #region 재정의

        // IEventDataGroupMetadata
        EventPropertyAttribute[] IEventDataGroupMetadata.PropertyDescriptorArray => propertyDescriptorArray;

        #endregion


        // 필드
        public readonly int minPropertyCount;
        public readonly EventPropertyAttribute[] propertyDescriptorArray;

        // 생성자
        public Metadata(Type propertyType) : base(propertyType)
        {
            this.propertyDescriptorArray = IEventDataContainerMetadata.LoadPropertyDescriptors(propertyType);

            // 프로퍼티 최소 개수
            this.minPropertyCount = 0;
            for (int i = 0; i < propertyDescriptorArray.Length; i++)
            {
                EventPropertyAttribute descriptor = propertyDescriptorArray[i];
                if (descriptor.isOptional == false)
                    minPropertyCount++;
            }
        }
    }

    #endregion
    
    // 생성자
    public EventObjectPropertyData(
        EventPropertyAttribute descriptor, ReadOnlyMemory<byte> data) : base(descriptor, data) { }
}