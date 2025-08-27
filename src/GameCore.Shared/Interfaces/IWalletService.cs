using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces
{
    public interface IWalletService
    {
        Task<WalletResult> GetBalanceAsync(int userId);
        Task<WalletResult> AddFundsAsync(int userId, decimal amount, string description = "充值");
        Task<WalletResult> WithdrawFundsAsync(int userId, decimal amount, string description = "扣款");
        Task<WalletResult> TransferFundsAsync(int fromUserId, TransferRequest request);
        Task<WalletTransactionListResult> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20);
    }
}
