namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 發送私訊請求 DTO
    /// </summary>
    public class SendMessageRequestDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// 聊天訊息響應 DTO
    /// </summary>
    public class ChatMessageResponseDto
    {
        public int MessageId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? ManagerName { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsSent { get; set; }
        public bool IsSystemMessage { get; set; }
    }

    /// <summary>
    /// 聊天對話響應 DTO
    /// </summary>
    public class ChatConversationResponseDto
    {
        public List<ChatMessageResponseDto> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int UnreadCount { get; set; }
    }

    /// <summary>
    /// 最近對話響應 DTO
    /// </summary>
    public class RecentConversationResponseDto
    {
        public int PeerId { get; set; }
        public string PeerName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsSystemMessage { get; set; }
    }

    /// <summary>
    /// 系統訊息請求 DTO
    /// </summary>
    public class SystemMessageRequestDto
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
    }

    /// <summary>
    /// 標記訊息已讀請求 DTO
    /// </summary>
    public class MarkMessageReadRequestDto
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
    }
}