using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

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
            .FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<UserWallet> CreateAsync(UserWallet wallet)
    {
        _context.UserWallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<UserWallet> UpdateAsync(UserWallet wallet)
    {
        wallet.UpdatedAt = DateTime.UtcNow;
        _context.UserWallets.Update(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<decimal> GetBalanceAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        return wallet?.Balance ?? 0m;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task AddTransactionAsync(WalletTransaction transaction)
    {
        await Task.Run(() => _context.WalletTransactions.Add(transaction));
    }

    public async Task<List<WalletTransaction>> GetTransactionsByUserIdAsync(int userId, int page = 1, int pageSize = 20)
    {
        return await _context.WalletTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTransactionCountByUserIdAsync(int userId)
    {
        return await _context.WalletTransactions
            .CountAsync(t => t.UserId == userId);
    }
}
