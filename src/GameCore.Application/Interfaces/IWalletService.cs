using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;

namespace GameCore.Application.Interfaces;

/// <summary>
/// 錢包服務介面
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// 取得使用者錢包餘額
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>錢包餘額</returns>
    Task<Result<WalletBalanceResponse>> GetBalanceAsync(int userId);
    
    /// <summary>
    /// 增加點數
    /// </summary>
    /// <param name="request">增加點數請求</param>
    /// <returns>操作結果</returns>
    Task<Result<WalletTransactionResponse>> AddPointsAsync(AddPointsRequest request);
    
    /// <summary>
    /// 扣除點數
    /// </summary>
    /// <param name="request">扣除點數請求</param>
    /// <returns>操作結果</returns>
    Task<Result<WalletTransactionResponse>> DeductPointsAsync(DeductPointsRequest request);
    
    /// <summary>
    /// 取得交易記錄
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易記錄列表</returns>
    Task<Result<WalletTransactionListResponse>> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20);
    
    /// <summary>
    /// 轉帳給其他使用者
    /// </summary>
    /// <param name="request">轉帳請求</param>
    /// <returns>轉帳結果</returns>
    Task<Result<TransferResponse>> TransferAsync(TransferRequest request);
} 