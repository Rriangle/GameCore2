using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶錢包資料存取層，處理用戶錢包相關的資料庫操作
/// </summary>
public class UserWalletRepository : IUserWalletRepository
{
    private readonly GameCoreDbContext _context;

    public UserWalletRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    public async Task<UserWallet?> GetByUserIdAsync(int userId)
    {
        return await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
    }

    public async Task<UserWallet> CreateAsync(UserWallet wallet)
    {
        _context.UserWallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<UserWallet> UpdateAsync(UserWallet wallet)
    {
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<int> GetPointsAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
        return wallet?.User_Point ?? 0;
    }

    public async Task<bool> UpdatePointsAsync(int userId, int points)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
            
        if (wallet == null)
        {
            return false;
        }
        
        wallet.User_Point = points;
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddPointsAsync(int userId, int pointsToAdd)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
            
        if (wallet == null)
        {
            return false;
        }
        
        wallet.User_Point += pointsToAdd;
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeductPointsAsync(int userId, int pointsToDeduct)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
            
        if (wallet == null || wallet.User_Point < pointsToDeduct)
        {
            return false;
        }
        
        wallet.User_Point -= pointsToDeduct;
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return true;
    }
}
