namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 聊天記錄回應 DTO
/// </summary>
public class ChatHistoryResponse
{
    /// <summary>
    /// 聊天記錄列表
    /// </summary>
    public List<ChatMessageResponse> Messages { get; set; } = new();
    
    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// 每頁大小
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// 聊天訊息回應 DTO
/// </summary>
public class ChatMessageResponse
{
    /// <summary>
    /// 訊息 ID
    /// </summary>
    public int MessageId { get; set; }
    
    /// <summary>
    /// 發送者 ID
    /// </summary>
    public int FromUserId { get; set; }
    
    /// <summary>
    /// 發送者名稱
    /// </summary>
    public string FromUserName { get; set; } = string.Empty;
    
    /// <summary>
    /// 訊息內容
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    public string MessageType { get; set; } = "text";
    
    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SendTime { get; set; }
    
    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; }
} 