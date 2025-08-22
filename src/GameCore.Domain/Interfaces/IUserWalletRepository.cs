using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

public interface IUserWalletRepository
{
    Task<UserWallet?> GetByUserIdAsync(int userId);
    Task<UserWallet> CreateAsync(UserWallet wallet);
    Task<UserWallet> UpdateAsync(UserWallet wallet);
    Task<decimal> GetBalanceAsync(int userId);
}
