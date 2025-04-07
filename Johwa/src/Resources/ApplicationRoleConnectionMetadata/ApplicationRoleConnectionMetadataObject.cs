using System.Text.Json;
using Johwa.Common.Json;
using Johwa.Utility;

namespace Johwa.Resources.ApplicationRoleConnectionMetadata;

/// <summary>
/// 애플리케이션의 역할 연결 메타데이터를 나타냅니다. <br/>
/// A representation of role connection metadata for an application.
/// </summary>
public struct ApplicationRoleConnectionMetadataObject : IJsonSource
{
    public JsonElement Property { get; set; }

    public ApplicationRoleConnectionMetadataObject(JsonElement applicationRoleConnectionMetadataObjectProperty)
    {
        this.Property = applicationRoleConnectionMetadataObjectProperty;
    }

    /// <summary>
    /// [ type ] <br/>
    /// 메타데이터 값의 유형 <br/>
    /// type of metadata value
    /// </summary>
    public ApplicationRoleConnectionMetadataType Type => (ApplicationRoleConnectionMetadataType)Property.GetProperty("type").GetInt32();

    /// <summary>
    /// [ key ] <br/>
    /// 메타데이터 필드의 딕셔너리 키 (a-z, 0-9, _ 문자; 1~50자) <br/>
    /// dictionary key for the metadata field (must be a-z, 0-9, or _ characters; 1-50 characters)
    /// </summary>
    public string Key => Property.GetProperty("key").GetString()!;

    /// <summary>
    /// [ name ] <br/>
    /// 메타데이터 필드의 이름 (1~100자) <br/>
    /// name of the metadata field (1-100 characters)
    /// </summary>
    public string Name => Property.GetProperty("name").GetString()!;

    /// <summary>
    /// [ name_localizations? ] <br/>
    /// 이름의 번역 <br/>
    /// translations of the name
    /// </summary>
    public JsonElementDictionaryResource? NameLocalizations
        => Property.FindJsonElementDictionaryOrNull("name_localizations");

    /// <summary>
    /// [ description ] <br/>
    /// 메타데이터 필드의 설명 (1~200자) <br/>
    /// description of the metadata field (1-200 characters)
    /// </summary>
    public string Description => Property.GetProperty("description").GetString()!;

    /// <summary>
    /// [ description_localizations? ] <br/>
    /// 설명의 번역 <br/>
    /// translations of the description
    /// </summary>
    public JsonElementDictionaryResource? DescriptionLocalizations
        => Property.FindJsonElementDictionaryOrNull("description_localizations");
}