using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 市場資料存取介面
/// </summary>
public interface IMarketRepository
{
    // PlayerMarketProductInfo 操作
    Task<PlayerMarketProductInfo?> GetMarketItemByIdAsync(int marketItemId);
    Task<List<PlayerMarketProductInfo>> GetMarketItemsAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null, int page = 1, int pageSize = 20);
    Task<int> GetMarketItemsCountAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null);
    Task<List<PlayerMarketProductInfo>> GetUserMarketItemsAsync(int userId, int page = 1, int pageSize = 20);
    Task<int> GetUserMarketItemsCountAsync(int userId);
    Task<PlayerMarketProductInfo> AddMarketItemAsync(PlayerMarketProductInfo marketItem);
    Task UpdateMarketItemAsync(PlayerMarketProductInfo marketItem);
    Task DeleteMarketItemAsync(int marketItemId);

    // PlayerMarketOrderInfo 操作
    Task<PlayerMarketOrderInfo?> GetMarketTransactionByIdAsync(int transactionId);
    Task<List<PlayerMarketOrderInfo>> GetMarketTransactionsAsync(int? buyerId = null, int? sellerId = null, string? status = null, int page = 1, int pageSize = 20);
    Task<int> GetMarketTransactionsCountAsync(int? buyerId = null, int? sellerId = null, string? status = null);
    Task<PlayerMarketOrderInfo> AddMarketTransactionAsync(PlayerMarketOrderInfo transaction);
    Task UpdateMarketTransactionAsync(PlayerMarketOrderInfo transaction);

    // 統計操作
    Task<int> GetTotalMarketItemsAsync();
    Task<int> GetActiveMarketItemsAsync();
    Task<int> GetTotalTransactionsAsync();
    Task<decimal> GetTotalTransactionVolumeAsync();

    // 搜尋操作
    Task<List<PlayerMarketProductInfo>> SearchMarketItemsAsync(MarketSearchRequest request);
    Task<int> SearchMarketItemsCountAsync(MarketSearchRequest request);

    // 通用操作
    Task SaveChangesAsync();
} 