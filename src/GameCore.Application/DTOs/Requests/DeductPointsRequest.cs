namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 扣除點數請求 DTO
/// </summary>
public class DeductPointsRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 點數數量
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// 原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// 來源類型
    /// </summary>
    public string SourceType { get; set; } = string.Empty;
    
    /// <summary>
    /// 來源 ID
    /// </summary>
    public string? SourceId { get; set; }
} 