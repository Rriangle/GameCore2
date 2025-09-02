namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 發送訊息回應 DTO
/// </summary>
public class SendMessageResponse
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
    /// 接收者 ID (私人訊息) 或群組 ID (群組訊息)
    /// </summary>
    public int TargetId { get; set; }
    
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
    /// 發送狀態
    /// </summary>
    public string Status { get; set; } = "sent";
} 