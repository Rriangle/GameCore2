using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces
{
    public interface IMarketRepository
    {
        // MarketItem 操作
        Task<MarketItem?> GetMarketItemByIdAsync(int marketItemId);
        Task<List<MarketItem>> GetMarketItemsAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null, int page = 1, int pageSize = 20);
        Task<int> GetMarketItemsCountAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null);
        Task<List<MarketItem>> GetUserMarketItemsAsync(int userId, int page = 1, int pageSize = 20);
        Task<int> GetUserMarketItemsCountAsync(int userId);
        Task<MarketItem> AddMarketItemAsync(MarketItem marketItem);
        Task UpdateMarketItemAsync(MarketItem marketItem);
        Task DeleteMarketItemAsync(int marketItemId);

        // MarketTransaction 操作
        Task<MarketTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<List<MarketTransaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20);
        Task<int> GetUserTransactionsCountAsync(int userId);
        Task<List<MarketTransaction>> GetMarketItemTransactionsAsync(int marketItemId);
        Task<MarketTransaction> AddTransactionAsync(MarketTransaction transaction);
        Task UpdateTransactionAsync(MarketTransaction transaction);

        // 統計操作
        Task<int> GetTotalMarketItemsAsync();
        Task<int> GetActiveMarketItemsAsync();
        Task<int> GetTotalTransactionsAsync();
        Task<decimal> GetTotalTransactionVolumeAsync();

        // 搜尋操作
        Task<List<MarketItem>> SearchItemsAsync(MarketSearchRequest request);

        // 通用操作
        Task SaveChangesAsync();
    }
}
