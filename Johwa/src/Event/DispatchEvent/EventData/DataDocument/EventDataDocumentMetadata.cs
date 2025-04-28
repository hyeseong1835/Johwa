using System.Runtime.InteropServices;
using Johwa.Common.Collection;

namespace Johwa.Event.Data;

public class EventDataDocumentMetadata
{
    #region Static

    static Dictionary<Type, EventDataDocumentMetadata> instanceDictionary = new ();
    public static EventDataDocumentMetadata GetOrCreateInstance(Type documentType)
    {
        if (instanceDictionary.TryGetValue(documentType, out EventDataDocumentMetadata? instance))
        {
            return instance;
        }
        else
        {
            EventDataDocumentMetadata newInstance = new (documentType);
            instanceDictionary.Add(documentType, newInstance);
            return newInstance;
        }
    }

    #endregion


    #region Instance
    
    public ReadOnlyByteSpanTree<EventDataInfo> dataInfoTree;

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        Marshal.OffsetOf(documentType, )
    }
    #endregion
}