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
}
