using System.Text.Json;
using Johwa.Common.Json;

namespace Johwa.Resources.Channel;

/// <summary>
/// 스레드의 메타데이터 구조 <br/>
/// Thread Metadata Structure
/// </summary>
public struct ThreadMetadata : IJsonSource
{
    public JsonElement Property { get; set; }

    public ThreadMetadata(JsonElement threadMetadataProperty)
    {
        this.Property = threadMetadataProperty;
    }

    /// <summary>
    /// [ archived ] <br/>
    /// 스레드가 보관되었는지 여부 <br/>
    /// whether the thread is archived
    /// </summary>
    public bool Archived => Property.GetProperty("archived").GetBoolean();

    /// <summary>
    /// [ auto_archive_duration ] <br/>
    /// 비활성 상태가 지속되면 스레드가 채널 목록에서 사라지는 시간 (분 단위) <br/>
    /// the thread will stop showing in the channel list after auto_archive_duration minutes of inactivity, can be set to: 60, 1440, 4320, 10080
    /// </summary>
    public int AutoArchiveDuration => Property.GetProperty("auto_archive_duration").GetInt32();

    /// <summary>
    /// [ archive_timestamp ] <br/>
    /// 스레드의 보관 상태가 마지막으로 변경된 시각 <br/>
    /// timestamp when the thread's archive status was last changed, used for calculating recent activity
    /// </summary>
    public DateTime ArchiveTimestamp => DateTime.Parse(Property.GetProperty("archive_timestamp").GetString()!);

    /// <summary>
    /// [ locked ] <br/>
    /// 스레드가 잠겨있는지 여부 <br/>
    /// whether the thread is locked; when a thread is locked, only users with MANAGE_THREADS can unarchive it
    /// </summary>
    public bool Locked => Property.GetProperty("locked").GetBoolean();

    /// <summary>
    /// [ invitable? ] <br/>
    /// 비모더레이터가 다른 비모더레이터를 스레드에 초대할 수 있는지 여부 (비공개 스레드 전용) <br/>
    /// whether non-moderators can add other non-moderators to a thread; only available on private threads
    /// </summary>
    public bool? Invitable
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("invitable", out prop) == false)
                return null;
                
            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            return prop.GetBoolean();
        }
    }

    /// <summary>
    /// [ create_timestamp? ] <br/>
    /// 스레드가 생성된 시각 (2022-01-09 이후 생성된 스레드에만 포함됨) <br/>
    /// timestamp when the thread was created; only populated for threads created after 2022-01-09
    /// </summary>
    public DateTime? CreateTimestamp
    {
        get
        {
            JsonElement prop;
            if (Property.TryGetProperty("create_timestamp", out prop) == false)
                return null;

            if (prop.ValueKind == JsonValueKind.Null)
                return null;

            return DateTime.TryParse(prop.GetString(), out var dt) ? dt : null;
        }
    }
}
