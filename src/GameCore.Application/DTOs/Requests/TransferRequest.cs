namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 轉帳請求 DTO
/// </summary>
public class TransferRequest
{
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
    /// 備註
    /// </summary>
    public string? Note { get; set; }
} 