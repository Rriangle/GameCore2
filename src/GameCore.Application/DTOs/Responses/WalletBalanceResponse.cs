namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 錢包餘額回應 DTO
/// </summary>
public class WalletBalanceResponse
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 點數餘額
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// 金幣餘額
    /// </summary>
    public int Coins { get; set; }
    
    /// <summary>
    /// 鑽石餘額
    /// </summary>
    public int Diamonds { get; set; }
    
    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
} 