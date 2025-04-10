using System.Reflection;

namespace Johwa.Event.Data;

public class DeferredParseObjectMetaData : DeferredParseMetaData
{
    public override EventDataAttribute Attribute => attribute;
    public readonly DeferredParseObjectAttribute attribute;

    public readonly EventDataMetadata[] propertyMetadataArray;
    public Type fieldType;
    
    public DeferredParseObjectMetaData(DeferredParseObjectAttribute attribute)
    { 
        this.attribute = attribute;
        fieldType = attribute.fieldInfo.FieldType;
        propertyMetadataArray = EventProperty.LoadMetadata(fieldType);
    }
    public override void InitProperty(object obj, IEventData container) { }
}

public class DeferredParseObjectAttribute : DeferredParseAttribute
{
    public readonly FieldInfo fieldInfo;

    public DeferredParseObjectAttribute(FieldInfo fieldInfo, 
        string name, bool isOptional = false) : base(name, isOptional) 
    { 
        this.fieldInfo = fieldInfo;  
    }
    public override EventDataMetadata CreateMetadata()
    {
        return new DeferredParseObjectMetaData(this);
    }
}
public abstract class DeferredParseObjectProperty : DeferredParseProperty
{
    public DeferredParseObjectProperty(
        EventData data) : base(data) { }

    public abstract void Init();
}