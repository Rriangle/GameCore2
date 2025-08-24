using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者錢包資料存取實作
/// </summary>
public class UserWalletRepository : IUserWalletRepository
{
    private readonly GameCoreDbContext _context;

    public UserWalletRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據使用者ID取得錢包資料
    /// </summary>
    public async Task<UserWallet?> GetByUserIdAsync(int userId)
    {
        return await _context.UserWallets
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    /// <summary>
    /// 創建錢包資料
    /// </summary>
    public async Task<UserWallet> CreateAsync(UserWallet wallet)
    {
        _context.UserWallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    /// <summary>
    /// 更新錢包資料
    /// </summary>
    public async Task<UserWallet> UpdateAsync(UserWallet wallet)
    {
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    /// <summary>
    /// 刪除錢包資料
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (wallet == null)
            return false;

        _context.UserWallets.Remove(wallet);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 取得使用者點數餘額
    /// </summary>
    public async Task<int> GetUserPointAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        return wallet?.UserPoint ?? 0;
    }

    /// <summary>
    /// 增加使用者點數
    /// </summary>
    public async Task<int> AddUserPointAsync(int userId, int points)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (wallet == null)
        {
            // 如果錢包不存在，創建一個新的
            wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = points,
                CouponNumber = null
            };
            _context.UserWallets.Add(wallet);
        }
        else
        {
            wallet.UserPoint += points;
        }
        
        await _context.SaveChangesAsync();
        return wallet.UserPoint;
    }

    /// <summary>
    /// 扣除使用者點數
    /// </summary>
    public async Task<int> DeductUserPointAsync(int userId, int points)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (wallet == null || wallet.UserPoint < points)
            return -1; // 餘額不足或錢包不存在

        wallet.UserPoint -= points;
        await _context.SaveChangesAsync();
        return wallet.UserPoint;
    }

    /// <summary>
    /// 設定優惠券編號
    /// </summary>
    public async Task<bool> SetCouponNumberAsync(int userId, string? couponNumber)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        
        if (wallet == null)
            return false;

        wallet.CouponNumber = couponNumber;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 取得使用者優惠券編號
    /// </summary>
    public async Task<string?> GetCouponNumberAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        return wallet?.CouponNumber;
    }

    /// <summary>
    /// 取得錢包餘額（舊版本相容）
    /// </summary>
    [Obsolete("請使用 GetUserPointAsync 方法")]
    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var points = await GetUserPointAsync(userId);
        return (decimal)points;
    }
}
