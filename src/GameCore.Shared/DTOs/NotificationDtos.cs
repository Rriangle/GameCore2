namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 創建通知請求 DTO
    /// </summary>
    public class CreateNotificationRequestDto
    {
        public int SourceId { get; set; }
        public int ActionId { get; set; }
        public int SenderId { get; set; }
        public int? SenderManagerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public List<int> RecipientUserIds { get; set; } = new();
    }

    /// <summary>
    /// 通知響應 DTO
    /// </summary>
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string? SenderManagerName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? GroupName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    /// <summary>
    /// 通知列表響應 DTO
    /// </summary>
    public class NotificationListResponseDto
    {
        public List<NotificationResponseDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int UnreadCount { get; set; }
    }

    /// <summary>
    /// 標記通知已讀請求 DTO
    /// </summary>
    public class MarkNotificationReadRequestDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// 通知來源響應 DTO
    /// </summary>
    public class NotificationSourceResponseDto
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 通知行為響應 DTO
    /// </summary>
    public class NotificationActionResponseDto
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 系統通知請求 DTO
    /// </summary>
    public class SystemNotificationRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<int> RecipientUserIds { get; set; } = new();
        public int? SenderManagerId { get; set; }
        public int? GroupId { get; set; }
    }
}