using System.Reflection;
using Johwa.Common.Collection;
using Johwa.Common.Extension.System;

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

    static ReadOnlyByteSpanTree<EventDataInfo> CreateDataInfoTree(Type documentType)
    {
        ReadOnlyByteSpanTree<EventDataInfo>.Builder treeBuilder = new ();

        // 필드 정보
        FieldInfo[] fieldInfoArray = documentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < fieldInfoArray.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfoArray[i];

            IEventField.ReadDocumentField(fieldInfo, ref treeBuilder);
        }

        return treeBuilder.BuildAndDispose();
    }

    #endregion


    #region Instance

    public ReadOnlyByteSpanTree<EventDataInfo> dataInfoTree; 

    // 생성자
    public EventDataDocumentMetadata(Type documentType)
    {
        this.dataInfoTree = CreateDataInfoTree(documentType);
    }
    #endregion
}