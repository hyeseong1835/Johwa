namespace Johwa.Event;

/// <summary>
/// 수신 이벤트 종류 <br/>
/// Types of events received from the Discord Gateway
/// </summary>
public enum DispatchEventType
{
    /// <summary>
    /// 하트비트 간격 정의 <br/>
    /// Defines the heartbeat interval
    /// </summary>
    HELLO,

    /// <summary>
    /// 초기 상태 정보 포함 <br/>
    /// Contains the initial state information
    /// </summary>
    READY,

    /// <summary>
    /// Resume에 대한 응답 <br/>
    /// Response to Resume
    /// </summary>
    RESUMED,

    /// <summary>
    /// 서버 종료, 클라이언트는 다시 연결 후 Resume 해야 함 <br/>
    /// Server is going away, client should reconnect to gateway and resume
    /// </summary>
    RECONNECT,

    /// <summary>
    /// Identify, Resume 실패 또는 세션이 유효하지 않음 <br/>
    /// Failure response to Identify or Resume or invalid active session
    /// </summary>
    INVALID_SESSION,

    /// <summary>
    /// 애플리케이션 명령어 권한이 업데이트됨 <br/>
    /// Application command permission was updated
    /// </summary>
    APPLICATION_COMMAND_PERMISSIONS_UPDATE,

    /// <summary>
    /// 자동 모더레이션 규칙이 생성됨 <br/>
    /// Auto Moderation rule was created
    /// </summary>
    AUTO_MODERATION_RULE_CREATE,

    /// <summary>
    /// 자동 모더레이션 규칙이 수정됨 <br/>
    /// Auto Moderation rule was updated
    /// </summary>
    AUTO_MODERATION_RULE_UPDATE,

    /// <summary>
    /// 자동 모더레이션 규칙이 삭제됨 <br/>
    /// Auto Moderation rule was deleted
    /// </summary>
    AUTO_MODERATION_RULE_DELETE,

    /// <summary>
    /// 자동 모더레이션 규칙이 트리거되고 작업이 실행됨 <br/>
    /// Auto Moderation rule was triggered and an action was executed
    /// </summary>
    AUTO_MODERATION_ACTION_EXECUTION,

    /// <summary>
    /// 길드 채널 생성됨 <br/>
    /// New guild channel created
    /// </summary>
    CHANNEL_CREATE,

    /// <summary>
    /// 채널이 수정됨 <br/>
    /// Channel was updated
    /// </summary>
    CHANNEL_UPDATE,

    /// <summary>
    /// 채널이 삭제됨 <br/>
    /// Channel was deleted
    /// </summary>
    CHANNEL_DELETE,

    /// <summary>
    /// 메시지 고정 또는 고정 해제됨 <br/>
    /// Message was pinned or unpinned
    /// </summary>
    CHANNEL_PINS_UPDATE,

    /// <summary>
    /// 쓰레드 생성됨 또는 비공개 쓰레드에 추가됨 <br/>
    /// Thread created or added to a private thread
    /// </summary>
    THREAD_CREATE,

    /// <summary>
    /// 쓰레드가 수정됨 <br/>
    /// Thread was updated
    /// </summary>
    THREAD_UPDATE,

    /// <summary>
    /// 쓰레드가 삭제됨 <br/>
    /// Thread was deleted
    /// </summary>
    THREAD_DELETE,

    /// <summary>
    /// 채널 접근 시 모든 활성 쓰레드 포함됨 <br/>
    /// All active threads in a channel when access is gained
    /// </summary>
    THREAD_LIST_SYNC,

    /// <summary>
    /// 현재 유저의 쓰레드 멤버 수정됨 <br/>
    /// Thread member for the current user was updated
    /// </summary>
    THREAD_MEMBER_UPDATE,

    /// <summary>
    /// 쓰레드에 유저가 추가 또는 제거됨 <br/>
    /// Some users were added or removed from a thread
    /// </summary>
    THREAD_MEMBERS_UPDATE,

    /// <summary>
    /// 권한이 생성됨 <br/>
    /// Entitlement was created
    /// </summary>
    ENTITLEMENT_CREATE,

    /// <summary>
    /// 권한이 갱신 또는 수정됨 <br/>
    /// Entitlement was updated or renewed
    /// </summary>
    ENTITLEMENT_UPDATE,

    /// <summary>
    /// 권한이 삭제됨 <br/>
    /// Entitlement was deleted
    /// </summary>
    ENTITLEMENT_DELETE,

    /// <summary>
    /// 길드 생성 또는 사용자 참여됨 <br/>
    /// Lazy-load for unavailable guild, guild became available, or user joined
    /// </summary>
    GUILD_CREATE,

    /// <summary>
    /// 길드가 수정됨 <br/>
    /// Guild was updated
    /// </summary>
    GUILD_UPDATE,

    /// <summary>
    /// 길드가 제거되었거나 유저가 떠남 <br/>
    /// Guild became unavailable or user left/was removed
    /// </summary>
    GUILD_DELETE,

    /// <summary>
    /// 감사 로그 항목이 생성됨 <br/>
    /// A guild audit log entry was created
    /// </summary>
    GUILD_AUDIT_LOG_ENTRY_CREATE,

    /// <summary>
    /// 유저가 차단됨 <br/>
    /// User was banned
    /// </summary>
    GUILD_BAN_ADD,

    /// <summary>
    /// 유저가 차단 해제됨 <br/>
    /// User was unbanned
    /// </summary>
    GUILD_BAN_REMOVE,

    /// <summary>
    /// 이모지가 수정됨 <br/>
    /// Guild emojis were updated
    /// </summary>
    GUILD_EMOJIS_UPDATE,

    /// <summary>
    /// 스티커가 수정됨 <br/>
    /// Guild stickers were updated
    /// </summary>
    GUILD_STICKERS_UPDATE,

    /// <summary>
    /// 통합이 수정됨 <br/>
    /// Guild integration was updated
    /// </summary>
    GUILD_INTEGRATIONS_UPDATE,

    /// <summary>
    /// 유저가 길드에 참가함 <br/>
    /// New user joined a guild
    /// </summary>
    GUILD_MEMBER_ADD,

    /// <summary>
    /// 유저가 길드에서 제거됨 <br/>
    /// User was removed from a guild
    /// </summary>
    GUILD_MEMBER_REMOVE,

    /// <summary>
    /// 길드 멤버가 수정됨 <br/>
    /// Guild member was updated
    /// </summary>
    GUILD_MEMBER_UPDATE,

    /// <summary>
    /// 멤버 목록 응답 <br/>
    /// Response to Request Guild Members
    /// </summary>
    GUILD_MEMBERS_CHUNK,

    /// <summary>
    /// 역할 생성됨 <br/>
    /// Guild role was created
    /// </summary>
    GUILD_ROLE_CREATE,

    /// <summary>
    /// 역할 수정됨 <br/>
    /// Guild role was updated
    /// </summary>
    GUILD_ROLE_UPDATE,

    /// <summary>
    /// 역할 삭제됨 <br/>
    /// Guild role was deleted
    /// </summary>
    GUILD_ROLE_DELETE,

    /// <summary>
    /// 스케줄 이벤트 생성됨 <br/>
    /// Guild scheduled event was created
    /// </summary>
    GUILD_SCHEDULED_EVENT_CREATE,

    /// <summary>
    /// 스케줄 이벤트 수정됨 <br/>
    /// Guild scheduled event was updated
    /// </summary>
    GUILD_SCHEDULED_EVENT_UPDATE,

    /// <summary>
    /// 스케줄 이벤트 삭제됨 <br/>
    /// Guild scheduled event was deleted
    /// </summary>
    GUILD_SCHEDULED_EVENT_DELETE,

    /// <summary>
    /// 유저가 스케줄 이벤트에 등록됨 <br/>
    /// User subscribed to a scheduled event
    /// </summary>
    GUILD_SCHEDULED_EVENT_USER_ADD,

    /// <summary>
    /// 유저가 스케줄 이벤트 등록 해제함 <br/>
    /// User unsubscribed from a scheduled event
    /// </summary>
    GUILD_SCHEDULED_EVENT_USER_REMOVE,

    /// <summary>
    /// 사운드보드 음향 생성됨 <br/>
    /// Guild soundboard sound was created
    /// </summary>
    GUILD_SOUNDBOARD_SOUND_CREATE,

    /// <summary>
    /// 사운드보드 음향 수정됨 <br/>
    /// Guild soundboard sound was updated
    /// </summary>
    GUILD_SOUNDBOARD_SOUND_UPDATE,

    /// <summary>
    /// 사운드보드 음향 삭제됨 <br/>
    /// Guild soundboard sound was deleted
    /// </summary>
    GUILD_SOUNDBOARD_SOUND_DELETE,

    /// <summary>
    /// 사운드보드 음향 목록이 수정됨 <br/>
    /// Guild soundboard sounds were updated
    /// </summary>
    GUILD_SOUNDBOARD_SOUNDS_UPDATE,

    /// <summary>
    /// 사운드보드 목록 응답 <br/>
    /// Response to Request Soundboard Sounds
    /// </summary>
    SOUNDBOARD_SOUNDS,

    /// <summary>
    /// 통합 생성됨 <br/>
    /// Guild integration was created
    /// </summary>
    INTEGRATION_CREATE,

    /// <summary>
    /// 통합 수정됨 <br/>
    /// Guild integration was updated
    /// </summary>
    INTEGRATION_UPDATE,

    /// <summary>
    /// 통합 삭제됨 <br/>
    /// Guild integration was deleted
    /// </summary>
    INTEGRATION_DELETE,

    /// <summary>
    /// 사용자 상호작용 발생 <br/>
    /// User used an interaction
    /// </summary>
    INTERACTION_CREATE,

    /// <summary>
    /// 초대가 생성됨 <br/>
    /// Invite to a channel was created
    /// </summary>
    INVITE_CREATE,

    /// <summary>
    /// 초대가 삭제됨 <br/>
    /// Invite to a channel was deleted
    /// </summary>
    INVITE_DELETE,

    /// <summary>
    /// 메시지가 생성됨 <br/>
    /// Message was created
    /// </summary>
    MESSAGE_CREATE,

    /// <summary>
    /// 메시지가 수정됨 <br/>
    /// Message was edited
    /// </summary>
    MESSAGE_UPDATE,

    /// <summary>
    /// 메시지가 삭제됨 <br/>
    /// Message was deleted
    /// </summary>
    MESSAGE_DELETE,

    /// <summary>
    /// 여러 메시지가 동시에 삭제됨 <br/>
    /// Multiple messages were deleted at once
    /// </summary>
    MESSAGE_DELETE_BULK,

    /// <summary>
    /// 메시지에 리액션 추가됨 <br/>
    /// User reacted to a message
    /// </summary>
    MESSAGE_REACTION_ADD,

    /// <summary>
    /// 메시지에서 리액션 제거됨 <br/>
    /// User removed a reaction from a message
    /// </summary>
    MESSAGE_REACTION_REMOVE,

    /// <summary>
    /// 메시지의 모든 리액션 제거됨 <br/>
    /// All reactions were removed from a message
    /// </summary>
    MESSAGE_REACTION_REMOVE_ALL,

    /// <summary>
    /// 특정 이모지의 모든 리액션 제거됨 <br/>
    /// All reactions for a given emoji were removed
    /// </summary>
    MESSAGE_REACTION_REMOVE_EMOJI,

    /// <summary>
    /// 사용자의 상태가 변경됨 <br/>
    /// User was updated
    /// </summary>
    PRESENCE_UPDATE,

    /// <summary>
    /// 무대 인스턴스 생성됨 <br/>
    /// Stage instance was created
    /// </summary>
    STAGE_INSTANCE_CREATE,

    /// <summary>
    /// 무대 인스턴스 수정됨 <br/>
    /// Stage instance was updated
    /// </summary>
    STAGE_INSTANCE_UPDATE,

    /// <summary>
    /// 무대 인스턴스 종료됨 <br/>
    /// Stage instance was deleted or closed
    /// </summary>
    STAGE_INSTANCE_DELETE,

    /// <summary>
    /// 프리미엄 앱 구독 생성됨 <br/>
    /// Premium App Subscription was created
    /// </summary>
    SUBSCRIPTION_CREATE,

    /// <summary>
    /// 프리미엄 앱 구독 수정됨 <br/>
    /// Premium App Subscription was updated
    /// </summary>
    SUBSCRIPTION_UPDATE,

    /// <summary>
    /// 프리미엄 앱 구독 삭제됨 <br/>
    /// Premium App Subscription was deleted
    /// </summary>
    SUBSCRIPTION_DELETE,

    /// <summary>
    /// 사용자가 입력을 시작함 <br/>
    /// User started typing in a channel
    /// </summary>
    TYPING_START,

    /// <summary>
    /// 사용자 정보가 변경됨 <br/>
    /// Properties about the user changed
    /// </summary>
    USER_UPDATE,

    /// <summary>
    /// 음성 채널에서 이펙트 전송됨 <br/>
    /// Someone sent an effect in a voice channel
    /// </summary>
    VOICE_CHANNEL_EFFECT_SEND,

    /// <summary>
    /// 음성 상태 변경됨 <br/>
    /// Someone joined, left, or moved a voice channel
    /// </summary>
    VOICE_STATE_UPDATE,

    /// <summary>
    /// 음성 서버가 업데이트됨 <br/>
    /// Guild's voice server was updated
    /// </summary>
    VOICE_SERVER_UPDATE,

    /// <summary>
    /// 웹후크가 생성, 수정 또는 삭제됨 <br/>
    /// Webhook was created, updated, or deleted
    /// </summary>
    WEBHOOKS_UPDATE,

    /// <summary>
    /// 투표가 추가됨 <br/>
    /// User voted on a poll
    /// </summary>
    MESSAGE_POLL_VOTE_ADD,

    /// <summary>
    /// 투표가 제거됨 <br/>
    /// User removed a vote on a poll
    /// </summary>
    MESSAGE_POLL_VOTE_REMOVE
}