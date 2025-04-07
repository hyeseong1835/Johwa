namespace Johwa.Resource.AuditLog;

/// <summary>
/// 감사 로그 이벤트 유형 <br/>
/// Type of action that occurred in the audit log
/// </summary>
public enum AuditLogEvent
{
    /// <summary>
    /// 서버 설정이 업데이트됨 <br/>
    /// Server settings were updated
    /// </summary>
    GuildUpdate = 1,

    /// <summary>
    /// 채널이 생성됨 <br/>
    /// Channel was created
    /// </summary>
    ChannelCreate = 10,

    /// <summary>
    /// 채널 설정이 업데이트됨 <br/>
    /// Channel settings were updated
    /// </summary>
    ChannelUpdate = 11,

    /// <summary>
    /// 채널이 삭제됨 <br/>
    /// Channel was deleted
    /// </summary>
    ChannelDelete = 12,

    /// <summary>
    /// 채널에 권한 덮어쓰기가 추가됨 <br/>
    /// Permission overwrite was added to a channel
    /// </summary>
    ChannelOverwriteCreate = 13,

    /// <summary>
    /// 채널에 권한 덮어쓰기가 수정됨 <br/>
    /// Permission overwrite was updated for a channel
    /// </summary>
    ChannelOverwriteUpdate = 14,

    /// <summary>
    /// 채널에서 권한 덮어쓰기가 삭제됨 <br/>
    /// Permission overwrite was deleted from a channel
    /// </summary>
    ChannelOverwriteDelete = 15,

    /// <summary>
    /// 멤버가 서버에서 추방됨 <br/>
    /// Member was removed from server
    /// </summary>
    MemberKick = 20,

    /// <summary>
    /// 멤버가 서버에서 정리됨 <br/>
    /// Members were pruned from server
    /// </summary>
    MemberPrune = 21,

    /// <summary>
    /// 멤버가 서버에서 차단됨 <br/>
    /// Member was banned from server
    /// </summary>
    MemberBanAdd = 22,

    /// <summary>
    /// 멤버의 서버 차단이 해제됨 <br/>
    /// Server ban was lifted for a member
    /// </summary>
    MemberBanRemove = 23,

    /// <summary>
    /// 멤버 정보가 업데이트됨 <br/>
    /// Member was updated in server
    /// </summary>
    MemberUpdate = 24,

    /// <summary>
    /// 멤버가 역할에서 추가/제거됨 <br/>
    /// Member was added or removed from a role
    /// </summary>
    MemberRoleUpdate = 25,

    /// <summary>
    /// 멤버가 음성 채널로 이동됨 <br/>
    /// Member was moved to a different voice channel
    /// </summary>
    MemberMove = 26,

    /// <summary>
    /// 멤버가 음성 채널에서 연결 해제됨 <br/>
    /// Member was disconnected from a voice channel
    /// </summary>
    MemberDisconnect = 27,

    /// <summary>
    /// 봇이 서버에 추가됨 <br/>
    /// Bot user was added to server
    /// </summary>
    BotAdd = 28,

    /// <summary>
    /// 역할이 생성됨 <br/>
    /// Role was created
    /// </summary>
    RoleCreate = 30,

    /// <summary>
    /// 역할이 수정됨 <br/>
    /// Role was edited
    /// </summary>
    RoleUpdate = 31,

    /// <summary>
    /// 역할이 삭제됨 <br/>
    /// Role was deleted
    /// </summary>
    RoleDelete = 32,

    /// <summary>
    /// 초대가 생성됨 <br/>
    /// Server invite was created
    /// </summary>
    InviteCreate = 40,

    /// <summary>
    /// 초대가 업데이트됨 <br/>
    /// Server invite was updated
    /// </summary>
    InviteUpdate = 41,

    /// <summary>
    /// 초대가 삭제됨 <br/>
    /// Server invite was deleted
    /// </summary>
    InviteDelete = 42,

    /// <summary>
    /// 웹훅이 생성됨 <br/>
    /// Webhook was created
    /// </summary>
    WebhookCreate = 50,

    /// <summary>
    /// 웹훅 속성 또는 채널이 수정됨 <br/>
    /// Webhook properties or channel were updated
    /// </summary>
    WebhookUpdate = 51,

    /// <summary>
    /// 웹훅이 삭제됨 <br/>
    /// Webhook was deleted
    /// </summary>
    WebhookDelete = 52,

    /// <summary>
    /// 이모지가 생성됨 <br/>
    /// Emoji was created
    /// </summary>
    EmojiCreate = 60,

    /// <summary>
    /// 이모지 이름이 업데이트됨 <br/>
    /// Emoji name was updated
    /// </summary>
    EmojiUpdate = 61,

    /// <summary>
    /// 이모지가 삭제됨 <br/>
    /// Emoji was deleted
    /// </summary>
    EmojiDelete = 62,

    /// <summary>
    /// 메시지가 삭제됨 <br/>
    /// Single message was deleted
    /// </summary>
    MessageDelete = 72,

    /// <summary>
    /// 다수의 메시지가 삭제됨 <br/>
    /// Multiple messages were deleted
    /// </summary>
    MessageBulkDelete = 73,

    /// <summary>
    /// 메시지가 고정됨 <br/>
    /// Message was pinned to a channel
    /// </summary>
    MessagePin = 74,

    /// <summary>
    /// 메시지 고정이 해제됨 <br/>
    /// Message was unpinned from a channel
    /// </summary>
    MessageUnpin = 75,

    /// <summary>
    /// 앱이 서버에 추가됨 <br/>
    /// App was added to server
    /// </summary>
    IntegrationCreate = 80,

    /// <summary>
    /// 앱이 수정됨 <br/>
    /// App was updated
    /// </summary>
    IntegrationUpdate = 81,

    /// <summary>
    /// 앱이 서버에서 제거됨 <br/>
    /// App was removed from server
    /// </summary>
    IntegrationDelete = 82,

    /// <summary>
    /// 스테이지 인스턴스가 생성됨 <br/>
    /// Stage instance was created
    /// </summary>
    StageInstanceCreate = 83,

    /// <summary>
    /// 스테이지 인스턴스 정보가 수정됨 <br/>
    /// Stage instance details were updated
    /// </summary>
    StageInstanceUpdate = 84,

    /// <summary>
    /// 스테이지 인스턴스가 종료됨 <br/>
    /// Stage instance was deleted
    /// </summary>
    StageInstanceDelete = 85,

    /// <summary>
    /// 스티커가 생성됨 <br/>
    /// Sticker was created
    /// </summary>
    StickerCreate = 90,

    /// <summary>
    /// 스티커가 수정됨 <br/>
    /// Sticker details were updated
    /// </summary>
    StickerUpdate = 91,

    /// <summary>
    /// 스티커가 삭제됨 <br/>
    /// Sticker was deleted
    /// </summary>
    StickerDelete = 92,

    /// <summary>
    /// 예정된 이벤트가 생성됨 <br/>
    /// Event was created
    /// </summary>
    GuildScheduledEventCreate = 100,

    /// <summary>
    /// 예정된 이벤트가 수정됨 <br/>
    /// Event was updated
    /// </summary>
    GuildScheduledEventUpdate = 101,

    /// <summary>
    /// 예정된 이벤트가 취소됨 <br/>
    /// Event was cancelled
    /// </summary>
    GuildScheduledEventDelete = 102,

    /// <summary>
    /// 스레드가 생성됨 <br/>
    /// Thread was created
    /// </summary>
    ThreadCreate = 110,

    /// <summary>
    /// 스레드가 수정됨 <br/>
    /// Thread was updated
    /// </summary>
    ThreadUpdate = 111,

    /// <summary>
    /// 스레드가 삭제됨 <br/>
    /// Thread was deleted
    /// </summary>
    ThreadDelete = 112,

    /// <summary>
    /// 명령어 권한이 수정됨 <br/>
    /// Permissions were updated for a command
    /// </summary>
    ApplicationCommandPermissionUpdate = 121,

    /// <summary>
    /// 사운드보드 사운드가 생성됨 <br/>
    /// Soundboard sound was created
    /// </summary>
    SoundboardSoundCreate = 130,

    /// <summary>
    /// 사운드보드 사운드가 수정됨 <br/>
    /// Soundboard sound was updated
    /// </summary>
    SoundboardSoundUpdate = 131,

    /// <summary>
    /// 사운드보드 사운드가 삭제됨 <br/>
    /// Soundboard sound was deleted
    /// </summary>
    SoundboardSoundDelete = 132,

    /// <summary>
    /// 자동 모더레이션 규칙이 생성됨 <br/>
    /// Auto Moderation rule was created
    /// </summary>
    AutoModerationRuleCreate = 140,

    /// <summary>
    /// 자동 모더레이션 규칙이 수정됨 <br/>
    /// Auto Moderation rule was updated
    /// </summary>
    AutoModerationRuleUpdate = 141,

    /// <summary>
    /// 자동 모더레이션 규칙이 삭제됨 <br/>
    /// Auto Moderation rule was deleted
    /// </summary>
    AutoModerationRuleDelete = 142,

    /// <summary>
    /// 메시지가 자동 모더레이션에 의해 차단됨 <br/>
    /// Message was blocked by Auto Moderation
    /// </summary>
    AutoModerationBlockMessage = 143,

    /// <summary>
    /// 메시지가 채널에 플래그됨 <br/>
    /// Message was flagged by Auto Moderation
    /// </summary>
    AutoModerationFlagToChannel = 144,

    /// <summary>
    /// 자동 모더레이션에 의해 타임아웃됨 <br/>
    /// Member was timed out by Auto Moderation
    /// </summary>
    AutoModerationUserCommunicationDisabled = 145,

    /// <summary>
    /// 창작자 수익화 요청 생성됨 <br/>
    /// Creator monetization request was created
    /// </summary>
    CreatorMonetizationRequestCreated = 150,

    /// <summary>
    /// 창작자 수익화 약관이 수락됨 <br/>
    /// Creator monetization terms were accepted
    /// </summary>
    CreatorMonetizationTermsAccepted = 151,

    /// <summary>
    /// 온보딩 질문 생성됨 <br/>
    /// Guild Onboarding Question was created
    /// </summary>
    OnboardingPromptCreate = 163,

    /// <summary>
    /// 온보딩 질문이 수정됨 <br/>
    /// Guild Onboarding Question was updated
    /// </summary>
    OnboardingPromptUpdate = 164,

    /// <summary>
    /// 온보딩 질문이 삭제됨 <br/>
    /// Guild Onboarding Question was deleted
    /// </summary>
    OnboardingPromptDelete = 165,

    /// <summary>
    /// 온보딩이 생성됨 <br/>
    /// Guild Onboarding was created
    /// </summary>
    OnboardingCreate = 166,

    /// <summary>
    /// 온보딩이 수정됨 <br/>
    /// Guild Onboarding was updated
    /// </summary>
    OnboardingUpdate = 167,

    /// <summary>
    /// 서버 가이드가 생성됨 <br/>
    /// Guild Server Guide was created
    /// </summary>
    HomeSettingsCreate = 190,

    /// <summary>
    /// 서버 가이드가 업데이트됨 <br/>
    /// Guild Server Guide was updated
    /// </summary>
    HomeSettingsUpdate = 191
}