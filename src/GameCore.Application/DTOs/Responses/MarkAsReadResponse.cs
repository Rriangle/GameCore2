namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 標記已讀回應 DTO
/// </summary>
public class MarkAsReadResponse
{
    /// <summary>
    /// 成功標記的訊息數量
    /// </summary>
    public int MarkedCount { get; set; }
    
    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 操作狀態
    /// </summary>
    public string Status { get; set; } = "success";
} 