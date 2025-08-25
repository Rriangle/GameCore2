namespace GameCore.Api.DTOs;

/// <summary>
/// 錢包餘額 DTO
/// </summary>
public class WalletBalanceDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 錢包餘額
    /// </summary>
    public int User_Point { get; set; }

    /// <summary>
    /// 優惠券號碼
    /// </summary>
    public string? Coupon_Number { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 錢包交易記錄 DTO
/// </summary>
public class WalletTransactionDto
{
    /// <summary>
    /// 交易編號
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 交易類型
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 交易金額
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 銷售錢包交易記錄 DTO
/// </summary>
public class SalesWalletTransactionDto
{
    /// <summary>
    /// 交易編號
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 交易類型
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 交易金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}