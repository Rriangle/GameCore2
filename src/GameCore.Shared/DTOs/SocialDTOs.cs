using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 通知 DTO
/// </summary>
public class NotificationDTO
{
    /// <summary>
    /// 通知ID
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// 來源類型ID
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 來源類型名稱
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 行為類型ID
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// 行為類型名稱
    /// </summary>
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// 發送者ID
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 通知標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知內容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 群組ID（若為群組相關）
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string? GroupName { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 已讀時間
    /// </summary>
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// 建立通知請求 DTO
/// </summary>
public class CreateNotificationRequest
{
    /// <summary>
    /// 來源類型ID
    /// </summary>
    [Required]
    public int SourceId { get; set; }

    /// <summary>
    /// 行為類型ID
    /// </summary>
    [Required]
    public int ActionId { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    [Required]
    public int SenderId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知內容
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 群組ID（可選）
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// 接收者ID列表
    /// </summary>
    [Required]
    public List<int> RecipientIds { get; set; } = new();
}

/// <summary>
/// 聊天訊息 DTO
/// </summary>
public class ChatMessageDTO
{
    /// <summary>
    /// 訊息ID
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    /// 管理員ID（客服）
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 接收者ID
    /// </summary>
    public int? ReceiverId { get; set; }

    /// <summary>
    /// 接收者名稱
    /// </summary>
    public string? ReceiverName { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// 是否已發送
    /// </summary>
    public bool IsSent { get; set; }
}

/// <summary>
/// 發送聊天訊息請求 DTO
/// </summary>
public class SendChatMessageRequest
{
    /// <summary>
    /// 管理員ID（可選，客服用）
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 接收者ID
    /// </summary>
    [Required]
    public int ReceiverId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 群組 DTO
/// </summary>
public class GroupDTO
{
    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 群組描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 建立者ID
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 成員數量
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 訊息數量
    /// </summary>
    public int MessageCount { get; set; }
}

/// <summary>
/// 建立群組請求 DTO
/// </summary>
public class CreateGroupRequest
{
    /// <summary>
    /// 群組名稱
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 創建者ID
    /// </summary>
    [Required]
    public int CreatedBy { get; set; }

    /// <summary>
    /// 初始成員ID列表
    /// </summary>
    public List<int> InitialMemberIds { get; set; } = new();
}

/// <summary>
/// 群組成員 DTO
/// </summary>
public class GroupMemberDTO
{
    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 用戶ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用戶名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 加入時間
    /// </summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>
    /// 是否為管理員
    /// </summary>
    public bool IsAdmin { get; set; }
}

/// <summary>
/// 群組聊天訊息 DTO
/// </summary>
public class GroupChatDTO
{
    /// <summary>
    /// 群組聊天ID
    /// </summary>
    public int GroupChatID { get; set; }

    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupID { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    public int SenderID { get; set; }

    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string GroupChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否已發送
    /// </summary>
    public bool IsSent { get; set; }
}

/// <summary>
/// 發送群組訊息請求 DTO
/// </summary>
public class SendGroupMessageRequest
{
    /// <summary>
    /// 群組ID
    /// </summary>
    [Required]
    public int GroupId { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    [Required]
    public int SenderId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 群組封鎖 DTO
/// </summary>
public class GroupBlockDTO
{
    /// <summary>
    /// 封鎖ID
    /// </summary>
    public int BlockId { get; set; }

    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 被封鎖者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 被封鎖者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 封鎖者ID
    /// </summary>
    public int BlockedBy { get; set; }

    /// <summary>
    /// 封鎖者名稱
    /// </summary>
    public string BlockedByName { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 封鎖用戶請求 DTO
/// </summary>
public class BlockUserRequest
{
    /// <summary>
    /// 群組ID
    /// </summary>
    [Required]
    public int GroupId { get; set; }

    /// <summary>
    /// 要封鎖的用戶ID
    /// </summary>
    [Required]
    public int UserId { get; set; }
}

/// <summary>
/// 通知統計 DTO
/// </summary>
public class NotificationStatsDTO
{
    /// <summary>
    /// 未讀通知數量
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 總通知數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 今日通知數量
    /// </summary>
    public int TodayCount { get; set; }
}

/// <summary>
/// 聊天統計 DTO
/// </summary>
public class ChatStatsDTO
{
    /// <summary>
    /// 未讀訊息數量
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 聊天對象數量
    /// </summary>
    public int ChatPartnerCount { get; set; }

    /// <summary>
    /// 今日訊息數量
    /// </summary>
    public int TodayMessageCount { get; set; }
}

/// <summary>
/// 群組統計 DTO
/// </summary>
public class GroupStatsDTO
{
    /// <summary>
    /// 加入的群組數量
    /// </summary>
    public int JoinedGroupCount { get; set; }

    /// <summary>
    /// 管理的群組數量
    /// </summary>
    public int AdminGroupCount { get; set; }

    /// <summary>
    /// 總群組數量
    /// </summary>
    public int TotalGroupCount { get; set; }
}

/// <summary>
/// 群組搜尋參數 DTO
/// </summary>
public class GroupSearchDto
{
    /// <summary>
    /// 搜尋關鍵字
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式
    /// </summary>
    public string SortBy { get; set; } = "created"; // "created", "name", "memberCount"

    /// <summary>
    /// 排序順序
    /// </summary>
    public string SortOrder { get; set; } = "desc"; // "asc", "desc"
}

/// <summary>
/// 群組詳細資訊 DTO
/// </summary>
public class GroupDetailDTO
{
    /// <summary>
    /// 群組ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 群組描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 建立者ID
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立者名稱
    /// </summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 成員數量
    /// </summary>
    public int MemberCount { get; set; }

    /// <summary>
    /// 是否為管理員
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 是否為成員
    /// </summary>
    public bool IsMember { get; set; }

    /// <summary>
    /// 成員列表
    /// </summary>
    public List<GroupMemberDTO> Members { get; set; } = new List<GroupMemberDTO>();
} 