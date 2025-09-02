namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 未讀訊息數量回應 DTO
/// </summary>
public class UnreadCountResponse
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 私人訊息未讀數量
    /// </summary>
    public int PrivateUnreadCount { get; set; }
    
    /// <summary>
    /// 群組訊息未讀數量
    /// </summary>
    public int GroupUnreadCount { get; set; }
    
    /// <summary>
    /// 總未讀數量
    /// </summary>
    public int TotalUnreadCount { get; set; }
    
    /// <summary>
    /// 查詢時間
    /// </summary>
    public DateTime QueryTime { get; set; } = DateTime.UtcNow;
} 