using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

public interface IUserWalletRepository
{
    Task<UserWallet?> GetByUserIdAsync(int userId);
    Task<UserWallet> CreateAsync(UserWallet wallet);
    Task<UserWallet> UpdateAsync(UserWallet wallet);
    Task<decimal> GetBalanceAsync(int userId);
    Task SaveChangesAsync();
    Task AddTransactionAsync(WalletTransaction transaction);
    Task<List<WalletTransaction>> GetTransactionsByUserIdAsync(int userId, int page = 1, int pageSize = 20);
    Task<int> GetTransactionCountByUserIdAsync(int userId);
}
