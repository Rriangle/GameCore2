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

/// <summary>
/// 錢包摘要 DTO - 新增：包含用戶錢包和銷售錢包的完整資訊
/// </summary>
public class WalletSummaryDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用戶錢包餘額
    /// </summary>
    public int UserWalletBalance { get; set; }

    /// <summary>
    /// 銷售錢包餘額
    /// </summary>
    public decimal SalesWalletBalance { get; set; }

    /// <summary>
    /// 銷售檔案狀態
    /// </summary>
    public string SalesProfileStatus { get; set; } = string.Empty;

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// 分頁錢包交易記錄 DTO - 新增：支援分頁的交易記錄
/// </summary>
public class PaginatedWalletTransactionsDto
{
    /// <summary>
    /// 交易記錄列表
    /// </summary>
    public List<WalletTransactionDto> Transactions { get; set; } = new();

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 頁面大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// 分頁銷售錢包交易記錄 DTO - 新增：支援分頁的銷售交易記錄
/// </summary>
public class PaginatedSalesWalletTransactionsDto
{
    /// <summary>
    /// 交易記錄列表
    /// </summary>
    public List<SalesWalletTransactionDto> Transactions { get; set; } = new();

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 頁面大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// 錢包統計資訊 DTO - 新增：錢包使用統計
/// </summary>
public class WalletStatisticsDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 本月交易次數
    /// </summary>
    public int MonthlyTransactionCount { get; set; }

    /// <summary>
    /// 本月交易總額
    /// </summary>
    public int MonthlyTransactionAmount { get; set; }

    /// <summary>
    /// 平均交易金額
    /// </summary>
    public decimal AverageTransactionAmount { get; set; }

    /// <summary>
    /// 最大單筆交易
    /// </summary>
    public int MaxTransactionAmount { get; set; }

    /// <summary>
    /// 統計期間
    /// </summary>
    public DateTime StatisticsPeriod { get; set; }
}

/// <summary>
/// 錢包操作請求 DTO - 新增：錢包操作（充值、消費等）
/// </summary>
public class WalletOperationRequestDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 操作類型
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 操作金額
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// 操作描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 參考編號（外部系統）
    /// </summary>
    public string? ReferenceNumber { get; set; }
}

/// <summary>
/// 錢包操作回應 DTO - 新增：錢包操作結果
/// </summary>
public class WalletOperationResponseDto
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 操作訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 操作前餘額
    /// </summary>
    public int PreviousBalance { get; set; }

    /// <summary>
    /// 操作後餘額
    /// </summary>
    public int NewBalance { get; set; }

    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime OperationTime { get; set; }

    /// <summary>
    /// 交易編號
    /// </summary>
    public string? TransactionId { get; set; }
}