namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 錢包交易回應 DTO
/// </summary>
public class WalletTransactionResponse
{
    /// <summary>
    /// 交易 ID
    /// </summary>
    public int TransactionId { get; set; }
    
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 交易類型
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;
    
    /// <summary>
    /// 點數變化
    /// </summary>
    public int PointsChange { get; set; }
    
    /// <summary>
    /// 交易後餘額
    /// </summary>
    public int BalanceAfter { get; set; }
    
    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime TransactionTime { get; set; }
    
    /// <summary>
    /// 交易原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
} 