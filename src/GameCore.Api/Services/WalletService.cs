using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Api.Services;

/// <summary>
/// 錢包服務
/// 處理使用者錢包相關業務邏輯
/// </summary>
public class WalletService : IWalletService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;

    public WalletService(IUserRepository userRepository, IUserWalletRepository walletRepository)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
    }

    /// <summary>
    /// 取得使用者錢包資訊
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>錢包資訊</returns>
    public async Task<WalletDto?> GetWalletAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null) return null;

        return new WalletDto
        {
            UserId = wallet.User_Id,
            Points = wallet.User_Point,
            Balance = wallet.User_Balance,
            LastUpdated = wallet.Last_Updated
        };
    }

    /// <summary>
    /// 增加使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> AddPointsAsync(int userId, int points, string reason)
    {
        if (points <= 0)
            return ServiceResult<bool>.CreateFailure("點數必須大於0");

        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            return ServiceResult<bool>.CreateFailure("錢包不存在");

        wallet.User_Point += points;
        wallet.Last_Updated = DateTime.UtcNow;

        await _walletRepository.UpdateAsync(wallet);

        // 記錄交易歷史
        await RecordTransactionAsync(userId, points, "add", reason);

        return ServiceResult<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// 扣除使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> DeductPointsAsync(int userId, int points, string reason)
    {
        if (points <= 0)
            return ServiceResult<bool>.CreateFailure("點數必須大於0");

        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            return ServiceResult<bool>.CreateFailure("錢包不存在");

        if (wallet.User_Point < points)
            return ServiceResult<bool>.CreateFailure("點數不足");

        wallet.User_Point -= points;
        wallet.Last_Updated = DateTime.UtcNow;

        await _walletRepository.UpdateAsync(wallet);

        // 記錄交易歷史
        await RecordTransactionAsync(userId, -points, "deduct", reason ?? string.Empty);

        return ServiceResult<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// 轉帳給其他使用者
    /// </summary>
    /// <param name="fromUserId">轉出使用者ID</param>
    /// <param name="toUserId">轉入使用者ID</param>
    /// <param name="points">點數</param>
    /// <param name="reason">原因</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> TransferPointsAsync(int fromUserId, int toUserId, int points, string reason)
    {
        if (points <= 0)
            return ServiceResult<bool>.CreateFailure("轉帳點數必須大於0");

        if (fromUserId == toUserId)
            return ServiceResult<bool>.CreateFailure("不能轉帳給自己");

        // 扣除轉出者點數
        var deductResult = await DeductPointsAsync(fromUserId, points, $"轉帳給使用者 {toUserId}: {reason}");
        if (!deductResult.Success)
            return ServiceResult<bool>.CreateFailure("轉帳失敗");

        // 增加轉入者點數
        var addResult = await AddPointsAsync(toUserId, points, $"收到使用者 {fromUserId} 轉帳: {reason}");
        if (!addResult.Success)
            return ServiceResult<bool>.CreateFailure("轉帳失敗");

        return ServiceResult<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// 取得交易歷史
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>交易歷史列表</returns>
    public async Task<PagedResult<TransactionDto>> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        var transactions = await _walletRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
        var total = await _walletRepository.GetTransactionCountByUserIdAsync(userId);

        var transactionDtos = transactions.Select(t => new TransactionDto
        {
            Id = t.TransactionID,
            UserId = t.UserID,
            Amount = t.Amount,
            Type = t.TransactionType,
            Reason = t.Description ?? string.Empty,
            CreatedAt = t.TransactionTime
        }).ToList();

        return new PagedResult<TransactionDto>
        {
            Data = transactionDtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    /// <summary>
    /// 記錄交易歷史
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="amount">金額</param>
    /// <param name="type">類型</param>
    /// <param name="reason">原因</param>
    private async Task RecordTransactionAsync(int userId, int amount, string type, string reason)
    {
        // 取得當前餘額
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        var currentBalance = wallet?.User_Point ?? 0;
        var newBalance = currentBalance + amount;

        var transaction = new UserWalletTransaction
        {
            UserID = userId,
            TransactionType = type,
            Amount = amount,
            BalanceBefore = currentBalance,
            BalanceAfter = newBalance,
            Description = reason ?? string.Empty,
            TransactionTime = DateTime.UtcNow
        };

        await _walletRepository.AddTransactionAsync(transaction);
    }
} 