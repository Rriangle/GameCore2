using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;
using GameCore.Application.Interfaces;

namespace GameCore.Application.Services;

/// <summary>
/// 錢包服務存根實作
/// </summary>
public class WalletService : IWalletService
{
    /// <summary>
    /// 取得使用者錢包餘額
    /// </summary>
    public Task<Result<WalletBalanceResponse>> GetBalanceAsync(int userId)
    {
        // TODO: 實作真實的餘額查詢邏輯
        throw new NotImplementedException("錢包餘額查詢功能尚未實作");
    }
    
    /// <summary>
    /// 增加點數
    /// </summary>
    public Task<Result<WalletTransactionResponse>> AddPointsAsync(AddPointsRequest request)
    {
        // TODO: 實作真實的點數增加邏輯
        throw new NotImplementedException("點數增加功能尚未實作");
    }
    
    /// <summary>
    /// 扣除點數
    /// </summary>
    public Task<Result<WalletTransactionResponse>> DeductPointsAsync(DeductPointsRequest request)
    {
        // TODO: 實作真實的點數扣除邏輯
        throw new NotImplementedException("點數扣除功能尚未實作");
    }
    
    /// <summary>
    /// 取得交易記錄
    /// </summary>
    public Task<Result<WalletTransactionListResponse>> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        // TODO: 實作真實的交易記錄查詢邏輯
        throw new NotImplementedException("交易記錄查詢功能尚未實作");
    }
    
    /// <summary>
    /// 轉帳給其他使用者
    /// </summary>
    public Task<Result<TransferResponse>> TransferAsync(TransferRequest request)
    {
        // TODO: 實作真實的轉帳邏輯
        throw new NotImplementedException("轉帳功能尚未實作");
    }
} 