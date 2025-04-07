namespace Johwa.Resources.Channel;

/// <summary>
/// 포럼 채널의 레이아웃 유형을 나타냅니다. <br/>
/// Types of layouts for forum channels.
/// </summary>
public enum ForumLayoutType
{
    /// <summary>
    /// 기본값이 설정되지 않음 <br/>
    /// No default has been set for forum channel
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// 게시글을 목록으로 표시 <br/>
    /// Display posts as a list
    /// </summary>
    ListView = 1,

    /// <summary>
    /// 게시글을 타일 형태로 표시 <br/>
    /// Display posts as a collection of tiles
    /// </summary>
    GalleryView = 2
}
