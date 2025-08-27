using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces
{
    public interface IMarketService
    {
        Task<MarketResult> ListItemAsync(ListItemRequest request);
        Task<MarketResult> BuyItemAsync(BuyItemRequest request);
        Task<MarketListResult> GetMarketItemsAsync(MarketSearchRequest request);
        Task<TransactionListResult> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20);
        Task<MarketResult> GetMarketStatisticsAsync();
        Task<MarketResult> CancelMarketItemAsync(int marketItemId, int userId);
        Task<MarketResult> UpdateItemAsync(int marketItemId, int userId, decimal newPrice, string newDescription);
        Task<MarketResult> SearchItemsAsync(MarketSearchRequest request);
        Task<MarketResult> CancelItemAsync(int marketItemId, int userId);
    }
}
