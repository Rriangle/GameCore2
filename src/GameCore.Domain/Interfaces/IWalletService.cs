using GameCore.Api.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 錢包服務介面
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>錢包餘額</returns>
    Task<WalletBalanceDto?> GetWalletBalanceAsync(int userId);

    /// <summary>
    /// 更新錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="amount">金額變化 (正數為增加，負數為減少)</param>
    /// <param name="reason">變化原因</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateWalletBalanceAsync(int userId, int amount, string reason);

    /// <summary>
    /// 取得錢包交易記錄
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易記錄</returns>
    Task<IEnumerable<WalletTransactionDto>> GetWalletTransactionsAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>銷售錢包餘額</returns>
    Task<SalesWalletDto?> GetSalesWalletBalanceAsync(int userId);

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="amount">金額變化</param>
    /// <param name="reason">變化原因</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateSalesWalletBalanceAsync(int userId, decimal amount, string reason);

    /// <summary>
    /// 取得銷售錢包交易記錄
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易記錄</returns>
    Task<IEnumerable<SalesWalletTransactionDto>> GetSalesWalletTransactionsAsync(int userId, int page = 1, int pageSize = 20);
}

/// <summary>
/// 錢包交易記錄 DTO
/// </summary>
public class WalletTransactionDto
{
    /// <summary>
    /// 交易 ID
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// 用戶 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 金額變化
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 變化原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime TransactionTime { get; set; }

    /// <summary>
    /// 交易後餘額
    /// </summary>
    public int BalanceAfter { get; set; }
}

/// <summary>
/// 銷售錢包交易記錄 DTO
/// </summary>
public class SalesWalletTransactionDto
{
    /// <summary>
    /// 交易 ID
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// 用戶 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 金額變化
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 變化原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime TransactionTime { get; set; }

    /// <summary>
    /// 交易後餘額
    /// </summary>
    public decimal BalanceAfter { get; set; }
}