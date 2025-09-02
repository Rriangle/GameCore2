using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 錢包資訊 DTO
/// </summary>
public class WalletDto
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者點數
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 使用者餘額
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// 交易記錄 DTO
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// 交易ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 交易類型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 交易原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 分頁結果 DTO
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 資料列表
    /// </summary>
    public List<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// 總數量
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每頁大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }
}
