using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces
{
    public interface IPlayerMarketService
    {
        Task<IEnumerable<PlayerMarketListing>> GetAllListingsAsync();
        Task<IEnumerable<PlayerMarketListing>> GetListingsByCategoryAsync(int categoryId);
        Task<IEnumerable<PlayerMarketListing>> GetUserListingsAsync(int userId);
        Task<PlayerMarketListing?> GetListingByIdAsync(int id);
        Task<PlayerMarketListing> CreateListingAsync(CreatePlayerMarketListingDto createListingDto);
        Task<PlayerMarketListing> UpdateListingAsync(int id, UpdatePlayerMarketListingDto updateListingDto);
        Task<bool> DeleteListingAsync(int id);
        Task<PlayerMarketOrder> CreateOrderAsync(CreatePlayerMarketOrderDto createOrderDto);
        Task<PlayerMarketOrder> ConfirmOrderAsync(int orderId);
        Task<PlayerMarketOrder> CompleteOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<IEnumerable<PlayerMarketOrder>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<PlayerMarketOrder>> GetUserSalesAsync(int userId);
        Task<decimal> CalculatePlatformFeeAsync(decimal amount);
    }
}