using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 錢包服務介面
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// 取得使用者錢包資訊
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>錢包資訊</returns>
    Task<WalletDto?> GetWalletAsync(int userId);

    /// <summary>
    /// 增加使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> AddPointsAsync(int userId, int points, string reason);

    /// <summary>
    /// 扣除使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> DeductPointsAsync(int userId, int points, string reason);

    /// <summary>
    /// 轉帳給其他使用者
    /// </summary>
    /// <param name="fromUserId">轉出使用者ID</param>
    /// <param name="toUserId">轉入使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> TransferPointsAsync(int fromUserId, int toUserId, int points, string reason);

    /// <summary>
    /// 取得交易歷史
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易歷史</returns>
    Task<GameCore.Shared.DTOs.PagedResult<TransactionDto>> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20);
} 