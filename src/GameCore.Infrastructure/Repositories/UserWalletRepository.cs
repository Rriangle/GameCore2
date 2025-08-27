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

    public async Task<int> GetBalanceAsync(int userId)
    {
        var wallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.User_Id == userId);
        return wallet?.User_Point ?? 0;
    }
}
