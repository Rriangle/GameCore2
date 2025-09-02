namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 發送群組訊息請求 DTO
/// </summary>
public class SendGroupMessageRequest
{
    /// <summary>
    /// 發送者 ID
    /// </summary>
    public int FromUserId { get; set; }
    
    /// <summary>
    /// 群組 ID
    /// </summary>
    public int GroupId { get; set; }
    
    /// <summary>
    /// 訊息內容
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 訊息類型
    /// </summary>
    public string MessageType { get; set; } = "text";
} 