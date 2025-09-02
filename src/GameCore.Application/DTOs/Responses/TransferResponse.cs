namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 轉帳回應 DTO
/// </summary>
public class TransferResponse
{
    /// <summary>
    /// 轉帳 ID
    /// </summary>
    public int TransferId { get; set; }
    
    /// <summary>
    /// 來源使用者 ID
    /// </summary>
    public int FromUserId { get; set; }
    
    /// <summary>
    /// 目標使用者 ID
    /// </summary>
    public int ToUserId { get; set; }
    
    /// <summary>
    /// 轉帳點數
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// 轉帳時間
    /// </summary>
    public DateTime TransferTime { get; set; }
    
    /// <summary>
    /// 轉帳狀態
    /// </summary>
    public string Status { get; set; } = "success";
    
    /// <summary>
    /// 備註
    /// </summary>
    public string? Note { get; set; }
} 