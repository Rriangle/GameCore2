namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 錢包交易列表回應 DTO
/// </summary>
public class WalletTransactionListResponse
{
    /// <summary>
    /// 交易列表
    /// </summary>
    public List<WalletTransactionResponse> Transactions { get; set; } = new();
    
    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// 每頁大小
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }
} 