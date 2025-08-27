using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

// ==================== 通知相關 DTOs ====================

/// <summary>
/// 通知 DTO
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// 通知編號
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// 來源編號
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 行為編號
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// 發送者編號
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 發送者管理員編號
    /// </summary>
    public int? SenderManagerId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    public string? NotificationMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 群組編號
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 已讀時間
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string? SourceName { get; set; }

    /// <summary>
    /// 行為名稱
    /// </summary>
    public string? ActionName { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string? GroupName { get; set; }
}

/// <summary>
/// 通知詳情 DTO
/// </summary>
public class NotificationDetailDto : NotificationDto
{
    /// <summary>
    /// 接收者列表
    /// </summary>
    public List<NotificationRecipientDto> Recipients { get; set; } = new();

    /// <summary>
    /// 額外資料 (JSON)
    /// </summary>
    public string? ExtraData { get; set; }
}

/// <summary>
/// 通知接收者 DTO
/// </summary>
public class NotificationRecipientDto
{
    /// <summary>
    /// 接收紀錄編號
    /// </summary>
    public int RecipientId { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 已讀時間
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }
}

/// <summary>
/// 通知來源 DTO
/// </summary>
public class NotificationSourceDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string SourceName { get; set; } = string.Empty;
}

/// <summary>
/// 通知行為 DTO
/// </summary>
public class NotificationActionDto
{
    /// <summary>
    /// 行為編號
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// 行為名稱
    /// </summary>
    public string ActionName { get; set; } = string.Empty;
}

/// <summary>
/// 分頁通知 DTO
/// </summary>
public class PagedNotificationsDto
{
    /// <summary>
    /// 通知列表
    /// </summary>
    public List<NotificationDto> Notifications { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

// ==================== 聊天相關 DTOs ====================

/// <summary>
/// 聊天訊息 DTO
/// </summary>
public class ChatMessageDto
{
    /// <summary>
    /// 訊息編號
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// 管理員編號
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 發送者編號
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 接收者編號
    /// </summary>
    public int? ReceiverId { get; set; }

    /// <summary>
    /// 聊天內容
    /// </summary>
    public string ChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 是否寄送
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 接收者名稱
    /// </summary>
    public string? ReceiverName { get; set; }

    /// <summary>
    /// 是否為當前用戶發送
    /// </summary>
    public bool IsCurrentUserSender { get; set; }
}

/// <summary>
/// 聊天聯絡人 DTO
/// </summary>
public class ChatContactDto
{
    /// <summary>
    /// 聯絡人編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 聯絡人名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡人暱稱
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 最後一則訊息
    /// </summary>
    public string? LastMessage { get; set; }

    /// <summary>
    /// 最後訊息時間
    /// </summary>
    public DateTime? LastMessageTime { get; set; }

    /// <summary>
    /// 未讀訊息數量
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 是否在線
    /// </summary>
    public bool IsOnline { get; set; }
}

/// <summary>
/// 分頁聊天訊息 DTO
/// </summary>
public class PagedChatMessagesDto
{
    /// <summary>
    /// 訊息列表
    /// </summary>
    public List<ChatMessageDto> Messages { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

// ==================== 群組相關 DTOs ====================

/// <summary>
/// 群組 DTO
/// </summary>
public class GroupDto
{
    /// <summary>
    /// 群組編號
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 建立者編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 建立者名稱
    /// </summary>
    public string? CreatorName { get; set; }

    /// <summary>
    /// 成員數量
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 是否為成員
    /// </summary>
    public bool IsMember { get; set; }

    /// <summary>
    /// 是否為管理員
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// 群組詳情 DTO
/// </summary>
public class GroupDetailDto : GroupDto
{
    /// <summary>
    /// 群組描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 群組設定
    /// </summary>
    public string? Settings { get; set; }

    /// <summary>
    /// 最近訊息列表
    /// </summary>
    public List<GroupChatMessageDto> RecentMessages { get; set; } = new();

    /// <summary>
    /// 成員列表預覽
    /// </summary>
    public List<GroupMemberDto> MemberPreview { get; set; } = new();
}

/// <summary>
/// 群組成員 DTO
/// </summary>
public class GroupMemberDto
{
    /// <summary>
    /// 群組編號
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 加入時間
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// 是否為管理員
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 使用者暱稱
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 是否在線
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActiveAt { get; set; }
}

/// <summary>
/// 群組聊天訊息 DTO
/// </summary>
public class GroupChatMessageDto
{
    /// <summary>
    /// 群組聊天編號
    /// </summary>
    public int GroupChatId { get; set; }

    /// <summary>
    /// 群組編號
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 發送者編號
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 群組聊天內容
    /// </summary>
    public string GroupChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否寄送
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// 發送者暱稱
    /// </summary>
    public string? SenderNickName { get; set; }

    /// <summary>
    /// 是否為當前用戶發送
    /// </summary>
    public bool IsCurrentUserSender { get; set; }
}

/// <summary>
/// 群組封鎖 DTO
/// </summary>
public class GroupBlockDto
{
    /// <summary>
    /// 封鎖編號
    /// </summary>
    public int BlockId { get; set; }

    /// <summary>
    /// 群組編號
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 被封鎖者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 封鎖者編號
    /// </summary>
    public int BlockedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 被封鎖者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 封鎖者名稱
    /// </summary>
    public string? BlockedByName { get; set; }

    /// <summary>
    /// 封鎖原因
    /// </summary>
    public string? Reason { get; set; }
}

// ==================== 分頁 DTOs ====================

/// <summary>
/// 分頁群組 DTO
/// </summary>
public class PagedGroupsDto
{
    /// <summary>
    /// 群組列表
    /// </summary>
    public List<GroupDto> Groups { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁群組成員 DTO
/// </summary>
public class PagedGroupMembersDto
{
    /// <summary>
    /// 成員列表
    /// </summary>
    public List<GroupMemberDto> Members { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁群組聊天訊息 DTO
/// </summary>
public class PagedGroupChatMessagesDto
{
    /// <summary>
    /// 訊息列表
    /// </summary>
    public List<GroupChatMessageDto> Messages { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁群組封鎖 DTO
/// </summary>
public class PagedGroupBlocksDto
{
    /// <summary>
    /// 封鎖列表
    /// </summary>
    public List<GroupBlockDto> Blocks { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

// ==================== 查詢 DTOs ====================

/// <summary>
/// 通知查詢 DTO
/// </summary>
public class NotificationQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 是否只顯示未讀
    /// </summary>
    public bool? UnreadOnly { get; set; }

    /// <summary>
    /// 來源篩選
    /// </summary>
    public int? SourceId { get; set; }

    /// <summary>
    /// 行為篩選
    /// </summary>
    public int? ActionId { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 聊天查詢 DTO
/// </summary>
public class ChatQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 排序方式 (newest/oldest)
    /// </summary>
    public string? SortBy { get; set; } = "newest";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 群組查詢 DTO
/// </summary>
public class GroupQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (newest/oldest/activity)
    /// </summary>
    public string? SortBy { get; set; } = "activity";
}

/// <summary>
/// 群組搜尋查詢 DTO
/// </summary>
public class GroupSearchQueryDto
{
    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (relevance/newest/members)
    /// </summary>
    public string? SortBy { get; set; } = "relevance";
}

/// <summary>
/// 群組成員查詢 DTO
/// </summary>
public class GroupMemberQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 是否只顯示管理員
    /// </summary>
    public bool? AdminOnly { get; set; }

    /// <summary>
    /// 排序方式 (newest/oldest/name)
    /// </summary>
    public string? SortBy { get; set; } = "newest";
}

/// <summary>
/// 群組聊天查詢 DTO
/// </summary>
public class GroupChatQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 排序方式 (newest/oldest)
    /// </summary>
    public string? SortBy { get; set; } = "newest";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 群組封鎖查詢 DTO
/// </summary>
public class GroupBlockQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (newest/oldest)
    /// </summary>
    public string? SortBy { get; set; } = "newest";
}

// ==================== 請求 DTOs ====================

/// <summary>
/// 建立系統通知請求 DTO
/// </summary>
public class CreateSystemNotificationRequestDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    [Required]
    public int SourceId { get; set; }

    /// <summary>
    /// 行為編號
    /// </summary>
    [Required]
    public int ActionId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [StringLength(200)]
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    [Required]
    public string NotificationMessage { get; set; } = string.Empty;

    /// <summary>
    /// 接收者編號列表
    /// </summary>
    [Required]
    public List<int> RecipientIds { get; set; } = new();

    /// <summary>
    /// 群組編號
    /// </summary>
    public int? GroupId { get; set; }
}

/// <summary>
/// 建立用戶通知請求 DTO
/// </summary>
public class CreateUserNotificationRequestDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    [Required]
    public int SourceId { get; set; }

    /// <summary>
    /// 行為編號
    /// </summary>
    [Required]
    public int ActionId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [StringLength(200)]
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    [Required]
    public string NotificationMessage { get; set; } = string.Empty;

    /// <summary>
    /// 接收者編號列表
    /// </summary>
    [Required]
    public List<int> RecipientIds { get; set; } = new();
}

/// <summary>
/// 建立群組通知請求 DTO
/// </summary>
public class CreateGroupNotificationRequestDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    [Required]
    public int SourceId { get; set; }

    /// <summary>
    /// 行為編號
    /// </summary>
    [Required]
    public int ActionId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [StringLength(200)]
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    [Required]
    public string NotificationMessage { get; set; } = string.Empty;
}

/// <summary>
/// 發送私訊請求 DTO
/// </summary>
public class SendDirectMessageRequestDto
{
    /// <summary>
    /// 接收者編號
    /// </summary>
    [Required]
    public int ReceiverId { get; set; }

    /// <summary>
    /// 聊天內容
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string ChatContent { get; set; } = string.Empty;
}

/// <summary>
/// 建立群組請求 DTO
/// </summary>
public class CreateGroupRequestDto
{
    /// <summary>
    /// 群組名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 群組描述
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 初始成員編號列表
    /// </summary>
    public List<int>? InitialMemberIds { get; set; }
}

/// <summary>
/// 發送群組訊息請求 DTO
/// </summary>
public class SendGroupMessageRequestDto
{
    /// <summary>
    /// 群組聊天內容
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string GroupChatContent { get; set; } = string.Empty;
}