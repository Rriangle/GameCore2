using Microsoft.EntityFrameworkCore;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;

namespace GameCore.Infrastructure.Repositories
{
    /// <summary>
    /// 玩家市場 Repository 實作
    /// 使用 PlayerMarketProductInfo 和 PlayerMarketOrderInfo 實體
    /// </summary>
    public class PlayerMarketRepository : IMarketRepository
    {
        private readonly GameCoreDbContext _context;

        public PlayerMarketRepository(GameCoreDbContext context)
        {
            _context = context;
        }

        // PlayerMarketProductInfo 操作
        public async Task<PlayerMarketProductInfo?> GetMarketItemByIdAsync(int marketItemId)
        {
            // 優化：使用 AsNoTracking() 提升唯讀查詢效能，避免 EF Core 變更追蹤開銷
            return await _context.PlayerMarketProductInfos
                .Include(m => m.Seller)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(m => m.PProductID == marketItemId);
        }

        public async Task<List<PlayerMarketProductInfo>> GetMarketItemsAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null, int page = 1, int pageSize = 20)
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            var query = _context.PlayerMarketProductInfos
                .Include(m => m.Seller)
                .AsNoTracking() // 效能優化：列表查詢通常不需要變更追蹤
                .Where(m => m.PStatus == "Active");

            if (productId.HasValue)
                query = query.Where(m => m.ProductID == productId.Value);

            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            if (sellerId.HasValue)
                query = query.Where(m => m.SellerID == sellerId.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetMarketItemsCountAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null)
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            var query = _context.PlayerMarketProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .Where(m => m.PStatus == "Active");

            if (productId.HasValue)
                query = query.Where(m => m.ProductID == productId.Value);

            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            if (sellerId.HasValue)
                query = query.Where(m => m.SellerID == sellerId.Value);

            return await query.CountAsync();
        }

        public async Task<List<PlayerMarketProductInfo>> GetUserMarketItemsAsync(int userId, int page = 1, int pageSize = 20)
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.PlayerMarketProductInfos
                .Include(m => m.Seller)
                .AsNoTracking() // 效能優化：列表查詢通常不需要變更追蹤
                .Where(m => m.SellerID == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserMarketItemsCountAsync(int userId)
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            return await _context.PlayerMarketProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .Where(m => m.SellerID == userId)
                .CountAsync();
        }

        public async Task<PlayerMarketProductInfo> AddMarketItemAsync(PlayerMarketProductInfo marketItem)
        {
            _context.PlayerMarketProductInfos.Add(marketItem);
            await _context.SaveChangesAsync();
            return marketItem;
        }

        public async Task UpdateMarketItemAsync(PlayerMarketProductInfo marketItem)
        {
            marketItem.UpdatedAt = DateTime.UtcNow;
            _context.PlayerMarketProductInfos.Update(marketItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMarketItemAsync(int marketItemId)
        {
            var marketItem = await _context.PlayerMarketProductInfos.FindAsync(marketItemId);
            if (marketItem != null)
            {
                _context.PlayerMarketProductInfos.Remove(marketItem);
                await _context.SaveChangesAsync();
            }
        }

        // PlayerMarketOrderInfo 操作
        public async Task<PlayerMarketOrderInfo?> GetMarketTransactionByIdAsync(int transactionId)
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.PlayerMarketOrderInfos
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(t => t.POrderID == transactionId);
        }

        public async Task<List<PlayerMarketOrderInfo>> GetMarketTransactionsAsync(int? buyerId = null, int? sellerId = null, string? status = null, int page = 1, int pageSize = 20)
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            var query = _context.PlayerMarketOrderInfos
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .AsNoTracking() // 效能優化：列表查詢通常不需要變更追蹤
                .AsQueryable();

            if (buyerId.HasValue)
                query = query.Where(t => t.BuyerID == buyerId.Value);

            if (sellerId.HasValue)
                query = query.Where(t => t.SellerID == sellerId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.POrderStatus == status);

            return await query
                .OrderByDescending(t => t.POrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetMarketTransactionsCountAsync(int? buyerId = null, int? sellerId = null, string? status = null)
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            var query = _context.PlayerMarketOrderInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .AsQueryable();

            if (buyerId.HasValue)
                query = query.Where(t => t.BuyerID == buyerId.Value);

            if (sellerId.HasValue)
                query = query.Where(t => t.SellerID == sellerId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.POrderStatus == status);

            return await query.CountAsync();
        }

        public async Task<PlayerMarketOrderInfo> AddMarketTransactionAsync(PlayerMarketOrderInfo transaction)
        {
            _context.PlayerMarketOrderInfos.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateMarketTransactionAsync(PlayerMarketOrderInfo transaction)
        {
            transaction.POrderUpdatedAt = DateTime.UtcNow;
            _context.PlayerMarketOrderInfos.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // 統計操作
        public async Task<int> GetTotalMarketItemsAsync()
        {
            // 效能優化：計數查詢不需要變更追蹤
            return await _context.PlayerMarketProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .CountAsync();
        }

        public async Task<int> GetActiveMarketItemsAsync()
        {
            // 效能優化：計數查詢不需要變更追蹤
            return await _context.PlayerMarketProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .Where(m => m.PStatus == "Active")
                .CountAsync();
        }

        public async Task<int> GetTotalTransactionsAsync()
        {
            // 效能優化：計數查詢不需要變更追蹤
            return await _context.PlayerMarketOrderInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .CountAsync();
        }

        public async Task<decimal> GetTotalTransactionVolumeAsync()
        {
            // 效能優化：彙總查詢不需要變更追蹤
            return await _context.PlayerMarketOrderInfos
                .AsNoTracking() // 效能優化：彙總查詢不需要變更追蹤
                .Where(t => t.POrderStatus == "Completed")
                .SumAsync(t => t.POrderTotal);
        }

        // 搜尋操作
        public async Task<List<PlayerMarketProductInfo>> SearchMarketItemsAsync(MarketSearchRequest request)
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            var query = _context.PlayerMarketProductInfos
                .Include(m => m.Seller)
                .AsNoTracking() // 效能優化：搜尋查詢通常不需要變更追蹤
                .Where(m => m.PStatus == "Active");

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(m => m.PProductTitle.Contains(request.Keyword) || 
                                        m.PProductName.Contains(request.Keyword) ||
                                        (m.PProductDescription != null && m.PProductDescription.Contains(request.Keyword)));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(m => m.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(m => m.Price <= request.MaxPrice.Value);

            if (request.SellerId.HasValue)
                query = query.Where(m => m.SellerID == request.SellerId.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<int> SearchMarketItemsCountAsync(MarketSearchRequest request)
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            var query = _context.PlayerMarketProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .Where(m => m.PStatus == "Active");

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(m => m.PProductTitle.Contains(request.Keyword) || 
                                        m.PProductName.Contains(request.Keyword) ||
                                        (m.PProductDescription != null && m.PProductDescription.Contains(request.Keyword)));
            }

            if (request.MinPrice.HasValue)
                query = query.Where(m => m.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(m => m.Price <= request.MaxPrice.Value);

            if (request.SellerId.HasValue)
                query = query.Where(m => m.SellerID == request.SellerId.Value);

            return await query.CountAsync();
        }

        // 通用操作
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
