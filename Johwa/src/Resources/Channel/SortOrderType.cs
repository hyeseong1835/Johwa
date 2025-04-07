namespace Johwa.Resources.Channel;

/// <summary>
/// 포럼 게시글 정렬 방식 <br/>
/// Sort forum posts by specific ordering method
/// </summary>
public enum SortOrderType
{
    /// <summary>
    /// 가장 최근 활동 순 <br/>
    /// Sort forum posts by activity
    /// </summary>
    LatestActivity = 0,

    /// <summary>
    /// 생성일 기준 정렬 (최신순) <br/>
    /// Sort forum posts by creation time (from most recent to oldest)
    /// </summary>
    CreationDate = 1
}