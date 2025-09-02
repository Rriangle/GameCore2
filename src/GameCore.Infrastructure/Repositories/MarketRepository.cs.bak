using Microsoft.EntityFrameworkCore;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;

namespace GameCore.Infrastructure.Repositories
{
    public class MarketRepository : IMarketRepository
    {
        private readonly GameCoreDbContext _context;

        public MarketRepository(GameCoreDbContext context)
        {
            _context = context;
        }

        // MarketItem 操作
        public async Task<MarketItem?> GetMarketItemByIdAsync(int marketItemId)
        {
            return await _context.MarketItems
                .Include(m => m.Seller)
                .FirstOrDefaultAsync(m => m.MarketItemId == marketItemId);
        }

        public async Task<List<MarketItem>> GetMarketItemsAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.MarketItems
                .Include(m => m.Seller)
                .Where(m => m.Status == MarketItemStatus.Active);

            if (productId.HasValue)
                query = query.Where(m => m.ProductId == productId.Value);

            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            if (sellerId.HasValue)
                query = query.Where(m => m.SellerId == sellerId.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetMarketItemsCountAsync(int? productId = null, decimal? minPrice = null, decimal? maxPrice = null, int? sellerId = null)
        {
            var query = _context.MarketItems.Where(m => m.Status == MarketItemStatus.Active);

            if (productId.HasValue)
                query = query.Where(m => m.ProductId == productId.Value);

            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            if (sellerId.HasValue)
                query = query.Where(m => m.SellerId == sellerId.Value);

            return await query.CountAsync();
        }

        public async Task<List<MarketItem>> GetUserMarketItemsAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.MarketItems
                .Include(m => m.Seller)
                .Where(m => m.SellerId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserMarketItemsCountAsync(int userId)
        {
            return await _context.MarketItems
                .Where(m => m.SellerId == userId)
                .CountAsync();
        }

        public async Task<MarketItem> AddMarketItemAsync(MarketItem marketItem)
        {
            _context.MarketItems.Add(marketItem);
            await _context.SaveChangesAsync();
            return marketItem;
        }

        public async Task UpdateMarketItemAsync(MarketItem marketItem)
        {
            marketItem.UpdatedAt = DateTime.UtcNow;
            _context.MarketItems.Update(marketItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMarketItemAsync(int marketItemId)
        {
            var marketItem = await _context.MarketItems.FindAsync(marketItemId);
            if (marketItem != null)
            {
                _context.MarketItems.Remove(marketItem);
                await _context.SaveChangesAsync();
            }
        }

        // MarketTransaction 操作
        public async Task<MarketTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.MarketTransactions
                .Include(t => t.MarketItem)
                .Include(t => t.Seller)
                .Include(t => t.Buyer)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<List<MarketTransaction>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.MarketTransactions
                .Include(t => t.MarketItem)
                .Include(t => t.Seller)
                .Include(t => t.Buyer)
                .Where(t => t.SellerId == userId || t.BuyerId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserTransactionsCountAsync(int userId)
        {
            return await _context.MarketTransactions
                .Where(t => t.SellerId == userId || t.BuyerId == userId)
                .CountAsync();
        }

        public async Task<List<MarketTransaction>> GetMarketItemTransactionsAsync(int marketItemId)
        {
            return await _context.MarketTransactions
                .Include(t => t.MarketItem)
                .Include(t => t.Seller)
                .Include(t => t.Buyer)
                .Where(t => t.MarketItemId == marketItemId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<MarketTransaction> AddTransactionAsync(MarketTransaction transaction)
        {
            _context.MarketTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateTransactionAsync(MarketTransaction transaction)
        {
            _context.MarketTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // 統計操作
        public async Task<int> GetTotalMarketItemsAsync()
        {
            return await _context.MarketItems.CountAsync();
        }

        public async Task<int> GetActiveMarketItemsAsync()
        {
            return await _context.MarketItems
                .Where(m => m.Status == MarketItemStatus.Active)
                .CountAsync();
        }

        public async Task<int> GetTotalTransactionsAsync()
        {
            return await _context.MarketTransactions.CountAsync();
        }

        public async Task<decimal> GetTotalTransactionVolumeAsync()
        {
            return await _context.MarketTransactions
                .Where(t => t.Status == MarketTransactionStatus.Completed)
                .SumAsync(t => t.TotalAmount);
        }

        // 搜尋操作
        public async Task<List<MarketItem>> SearchItemsAsync(MarketSearchRequest request)
        {
            var query = _context.MarketItems
                .Include(m => m.Seller)
                .Where(m => m.Status == MarketItemStatus.Active);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(m => m.Description.Contains(request.Keyword));
            }

            if (request.ProductId.HasValue)
                query = query.Where(m => m.ProductId == request.ProductId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(m => m.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(m => m.Price <= request.MaxPrice.Value);

            if (request.SellerId.HasValue)
                query = query.Where(m => m.SellerId == request.SellerId.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        // 通用操作
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
