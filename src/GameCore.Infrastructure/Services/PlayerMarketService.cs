using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 玩家市場服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class PlayerMarketService : IPlayerMarketService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PlayerMarketService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 5;
    private const string AllListingsCacheKey = "PlayerMarket_AllListings";
    private const string UserListingsCacheKey = "PlayerMarket_UserListings_{0}";
    private const string ListingCacheKey = "PlayerMarket_Listing_{0}";
    private const string UserOrdersCacheKey = "PlayerMarket_UserOrders_{0}";
    private const string UserSalesCacheKey = "PlayerMarket_UserSales_{0}";
    private const decimal PlatformFeeRate = 0.05m; // 5% platform fee
    private const decimal MinimumPlatformFee = 1.00m; // Minimum $1 fee
    private const int DefaultListingExpiryDays = 30;

    public PlayerMarketService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<PlayerMarketService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取所有掛牌 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<PlayerMarketListing>> GetAllListingsAsync()
    {
        _logger.LogInformation("開始獲取所有玩家市場掛牌");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(AllListingsCacheKey, out IEnumerable<PlayerMarketListing> cachedListings))
            {
                _logger.LogDebug("從快取獲取所有掛牌，數量: {Count}", cachedListings.Count());
                return cachedListings;
            }

            // 從資料庫獲取
            var listings = await _context.PlayerMarketListings
                .Include(l => l.Seller)
                .Where(l => l.Status == ListingStatus.Active)
                .OrderByDescending(l => l.CreatedAt)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(AllListingsCacheKey, listings, cacheOptions);

            _logger.LogInformation("成功獲取所有掛牌，數量: {Count}", listings.Count);
            return listings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取所有掛牌時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 根據分類獲取掛牌 - 優化版本
    /// </summary>
    public async Task<IEnumerable<PlayerMarketListing>> GetListingsByCategoryAsync(int categoryId)
    {
        _logger.LogInformation("開始根據分類獲取掛牌，分類ID: {CategoryId}", categoryId);

        try
        {
            // 輸入驗證
            if (categoryId <= 0)
            {
                _logger.LogWarning("無效的分類ID: {CategoryId}", categoryId);
                return Enumerable.Empty<PlayerMarketListing>();
            }

            // 注意：這需要與玩家市場的分類系統一起實現
            // 目前返回所有活躍掛牌
            var allListings = await GetAllListingsAsync();
            
            // TODO: 實現分類篩選邏輯
            _logger.LogInformation("成功獲取分類掛牌，分類ID: {CategoryId}, 數量: {Count}", categoryId, allListings.Count());
            return allListings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據分類獲取掛牌時發生錯誤，分類ID: {CategoryId}", categoryId);
            throw;
        }
    }

    /// <summary>
    /// 獲取用戶掛牌 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<PlayerMarketListing>> GetUserListingsAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶掛牌，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<PlayerMarketListing>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserListingsCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<PlayerMarketListing> cachedListings))
            {
                _logger.LogDebug("從快取獲取用戶掛牌，用戶ID: {UserId}, 數量: {Count}", userId, cachedListings.Count());
                return cachedListings;
            }

            // 從資料庫獲取
            var listings = await _context.PlayerMarketListings
                .Where(l => l.SellerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, listings, cacheOptions);

            _logger.LogInformation("成功獲取用戶掛牌，用戶ID: {UserId}, 數量: {Count}", userId, listings.Count);
            return listings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶掛牌時發生錯誤，用戶ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// 根據ID獲取掛牌 - 優化版本（含快取）
    /// </summary>
    public async Task<PlayerMarketListing?> GetListingByIdAsync(int id)
    {
        _logger.LogInformation("開始根據ID獲取掛牌，掛牌ID: {ListingId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的掛牌ID: {ListingId}", id);
                return null;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ListingCacheKey, id);
            if (_memoryCache.TryGetValue(cacheKey, out PlayerMarketListing cachedListing))
            {
                _logger.LogDebug("從快取獲取掛牌，掛牌ID: {ListingId}", id);
                return cachedListing;
            }

            // 從資料庫獲取
            var listing = await _context.PlayerMarketListings
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (listing != null)
            {
                // 存入快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, listing, cacheOptions);
            }

            _logger.LogInformation("成功獲取掛牌，掛牌ID: {ListingId}", id);
            return listing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據ID獲取掛牌時發生錯誤，掛牌ID: {ListingId}", id);
            throw;
        }
    }

    /// <summary>
    /// 創建掛牌 - 優化版本
    /// </summary>
    public async Task<PlayerMarketListing> CreateListingAsync(CreatePlayerMarketListingDto createListingDto)
    {
        _logger.LogInformation("開始創建掛牌，賣家ID: {SellerId}", createListingDto.SellerId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateListingRequest(createListingDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("創建掛牌請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var listing = new PlayerMarketListing
            {
                SellerId = createListingDto.SellerId,
                Title = createListingDto.Title?.Trim(),
                Description = createListingDto.Description?.Trim(),
                Price = createListingDto.Price,
                Quantity = createListingDto.Quantity,
                AvailableQuantity = createListingDto.Quantity,
                Status = ListingStatus.Active,
                ImageUrl = createListingDto.ImageUrl?.Trim(),
                IsNegotiable = createListingDto.IsNegotiable,
                ExpiresAt = createListingDto.ExpiresAt ?? DateTime.UtcNow.AddDays(DefaultListingExpiryDays),
                CreatedAt = DateTime.UtcNow
            };

            _context.PlayerMarketListings.Add(listing);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPlayerMarketRelatedCache(createListingDto.SellerId);

            _logger.LogInformation("成功創建掛牌，掛牌ID: {ListingId}, 標題: {Title}", listing.Id, listing.Title);
            return listing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建掛牌時發生錯誤，賣家ID: {SellerId}", createListingDto.SellerId);
            throw;
        }
    }

    /// <summary>
    /// 更新掛牌 - 優化版本
    /// </summary>
    public async Task<PlayerMarketListing> UpdateListingAsync(int id, UpdatePlayerMarketListingDto updateListingDto)
    {
        _logger.LogInformation("開始更新掛牌，掛牌ID: {ListingId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的掛牌ID: {ListingId}", id);
                throw new ArgumentException("無效的掛牌ID");
            }

            var listing = await _context.PlayerMarketListings.FindAsync(id);
            if (listing == null)
            {
                _logger.LogWarning("掛牌不存在，掛牌ID: {ListingId}", id);
                throw new ArgumentException("掛牌不存在");
            }

            // 驗證更新請求
            var validationResult = ValidateUpdateListingRequest(updateListingDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("更新掛牌請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 更新掛牌欄位
            UpdateListingFields(listing, updateListingDto);
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPlayerMarketRelatedCache(listing.SellerId);

            _logger.LogInformation("成功更新掛牌，掛牌ID: {ListingId}", id);
            return listing;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新掛牌時發生錯誤，掛牌ID: {ListingId}", id);
            throw;
        }
    }

    /// <summary>
    /// 刪除掛牌 - 優化版本
    /// </summary>
    public async Task<bool> DeleteListingAsync(int id)
    {
        _logger.LogInformation("開始刪除掛牌，掛牌ID: {ListingId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的掛牌ID: {ListingId}", id);
                return false;
            }

            var listing = await _context.PlayerMarketListings.FindAsync(id);
            if (listing == null)
            {
                _logger.LogWarning("掛牌不存在，掛牌ID: {ListingId}", id);
                return false;
            }

            listing.Status = ListingStatus.Cancelled;
            listing.CancelledAt = DateTime.UtcNow;
            listing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPlayerMarketRelatedCache(listing.SellerId);

            _logger.LogInformation("成功刪除掛牌，掛牌ID: {ListingId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除掛牌時發生錯誤，掛牌ID: {ListingId}", id);
            return false;
        }
    }

    /// <summary>
    /// 創建訂單 - 優化版本
    /// </summary>
    public async Task<PlayerMarketOrder> CreateOrderAsync(CreatePlayerMarketOrderDto createOrderDto)
    {
        _logger.LogInformation("開始創建玩家市場訂單，掛牌ID: {ListingId}, 買家ID: {BuyerId}", 
            createOrderDto.ListingId, createOrderDto.BuyerId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateOrderRequest(createOrderDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("創建訂單請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 使用事務確保資料一致性
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var listing = await _context.PlayerMarketListings.FindAsync(createOrderDto.ListingId);
                if (listing == null)
                    throw new ArgumentException("掛牌不存在");

                if (listing.Status != ListingStatus.Active)
                    throw new InvalidOperationException("掛牌不活躍");

                if (listing.AvailableQuantity < createOrderDto.Quantity)
                    throw new InvalidOperationException("可用數量不足");

                var seller = await _context.Users.FindAsync(listing.SellerId);
                if (seller == null)
                    throw new ArgumentException("賣家不存在");

                var buyer = await _context.Users.FindAsync(createOrderDto.BuyerId);
                if (buyer == null)
                    throw new ArgumentException("買家不存在");

                if (createOrderDto.BuyerId == listing.SellerId)
                    throw new InvalidOperationException("買家不能是賣家");

                var totalAmount = listing.Price * createOrderDto.Quantity;
                var platformFee = await CalculatePlatformFeeAsync(totalAmount);
                var sellerAmount = totalAmount - platformFee;

                var order = new PlayerMarketOrder
                {
                    ListingId = createOrderDto.ListingId,
                    BuyerId = createOrderDto.BuyerId,
                    SellerId = listing.SellerId,
                    Quantity = createOrderDto.Quantity,
                    UnitPrice = listing.Price,
                    TotalAmount = totalAmount,
                    PlatformFee = platformFee,
                    SellerAmount = sellerAmount,
                    Status = PlayerMarketOrderStatus.Pending,
                    BuyerNotes = createOrderDto.BuyerNotes?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.PlayerMarketOrders.Add(order);

                // 更新掛牌可用數量
                UpdateListingQuantity(listing, createOrderDto.Quantity);

                await transaction.CommitAsync();

                // 清除相關快取
                ClearPlayerMarketRelatedCache(listing.SellerId);
                ClearPlayerMarketRelatedCache(createOrderDto.BuyerId);

                _logger.LogInformation("成功創建玩家市場訂單，訂單ID: {OrderId}, 金額: {Amount}", order.Id, order.TotalAmount);
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建玩家市場訂單時發生錯誤，掛牌ID: {ListingId}", createOrderDto.ListingId);
            throw;
        }
    }

    /// <summary>
    /// 確認訂單 - 優化版本
    /// </summary>
    public async Task<PlayerMarketOrder> ConfirmOrderAsync(int orderId)
    {
        _logger.LogInformation("開始確認玩家市場訂單，訂單ID: {OrderId}", orderId);

        try
        {
            // 輸入驗證
            if (orderId <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", orderId);
                throw new ArgumentException("無效的訂單ID");
            }

            var order = await _context.PlayerMarketOrders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", orderId);
                throw new ArgumentException("訂單不存在");
            }

            if (order.Status != PlayerMarketOrderStatus.Pending)
            {
                _logger.LogWarning("訂單無法在當前狀態確認，訂單ID: {OrderId}, 狀態: {Status}", orderId, order.Status);
                throw new InvalidOperationException("訂單無法在當前狀態確認");

            }

            order.Status = PlayerMarketOrderStatus.Confirmed;
            order.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPlayerMarketRelatedCache(order.SellerId);
            ClearPlayerMarketRelatedCache(order.BuyerId);

            _logger.LogInformation("成功確認玩家市場訂單，訂單ID: {OrderId}", orderId);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "確認玩家市場訂單時發生錯誤，訂單ID: {OrderId}", orderId);
            throw;
        }
    }

    /// <summary>
    /// 完成訂單 - 優化版本
    /// </summary>
    public async Task<PlayerMarketOrder> CompleteOrderAsync(int orderId)
    {
        _logger.LogInformation("開始完成玩家市場訂單，訂單ID: {OrderId}", orderId);

        try
        {
            // 輸入驗證
            if (orderId <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", orderId);
                throw new ArgumentException("無效的訂單ID");
            }

            var order = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", orderId);
                throw new ArgumentException("訂單不存在");
            }

            if (order.Status != PlayerMarketOrderStatus.Confirmed)
            {
                _logger.LogWarning("訂單無法在當前狀態完成，訂單ID: {OrderId}, 狀態: {Status}", orderId, order.Status);
                throw new InvalidOperationException("訂單無法在當前狀態完成");
            }

            order.Status = PlayerMarketOrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;

            // 更新掛牌狀態（如果已售完）
            if (order.Listing.Status == ListingStatus.Sold)
            {
                order.Listing.Status = ListingStatus.Sold;
                order.Listing.SoldAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPlayerMarketRelatedCache(order.SellerId);
            ClearPlayerMarketRelatedCache(order.BuyerId);

            _logger.LogInformation("成功完成玩家市場訂單，訂單ID: {OrderId}", orderId);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成玩家市場訂單時發生錯誤，訂單ID: {OrderId}", orderId);
            throw;
        }
    }

    /// <summary>
    /// 取消訂單 - 優化版本
    /// </summary>
    public async Task<bool> CancelOrderAsync(int orderId)
    {
        _logger.LogInformation("開始取消玩家市場訂單，訂單ID: {OrderId}", orderId);

        try
        {
            // 輸入驗證
            if (orderId <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", orderId);
                return false;
            }

            var order = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", orderId);
                return false;
            }

            if (order.Status != PlayerMarketOrderStatus.Pending)
            {
                _logger.LogWarning("訂單無法在當前狀態取消，訂單ID: {OrderId}, 狀態: {Status}", orderId, order.Status);
                return false;
            }

            // 使用事務確保資料一致性
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                order.Status = PlayerMarketOrderStatus.Cancelled;
                order.CancelledAt = DateTime.UtcNow;

                // 恢復掛牌可用數量
                RestoreListingQuantity(order.Listing, order.Quantity);

                await transaction.CommitAsync();

                // 清除相關快取
                ClearPlayerMarketRelatedCache(order.SellerId);
                ClearPlayerMarketRelatedCache(order.BuyerId);

                _logger.LogInformation("成功取消玩家市場訂單，訂單ID: {OrderId}", orderId);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消玩家市場訂單時發生錯誤，訂單ID: {OrderId}", orderId);
            return false;
        }
    }

    /// <summary>
    /// 獲取用戶訂單 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<PlayerMarketOrder>> GetUserOrdersAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶訂單，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<PlayerMarketOrder>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserOrdersCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<PlayerMarketOrder> cachedOrders))
            {
                _logger.LogDebug("從快取獲取用戶訂單，用戶ID: {UserId}, 數量: {Count}", userId, cachedOrders.Count());
                return cachedOrders;
            }

            // 從資料庫獲取
            var orders = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .Include(o => o.Seller)
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, orders, cacheOptions);

            _logger.LogInformation("成功獲取用戶訂單，用戶ID: {UserId}, 數量: {Count}", userId, orders.Count);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶訂單時發生錯誤，用戶ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// 獲取用戶銷售 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<PlayerMarketOrder>> GetUserSalesAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶銷售，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<PlayerMarketOrder>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserSalesCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<PlayerMarketOrder> cachedSales))
            {
                _logger.LogDebug("從快取獲取用戶銷售，用戶ID: {UserId}, 數量: {Count}", userId, cachedSales.Count());
                return cachedSales;
            }

            // 從資料庫獲取
            var sales = await _context.PlayerMarketOrders
                .Include(o => o.Listing)
                .Include(o => o.Buyer)
                .Where(o => o.SellerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, sales, cacheOptions);

            _logger.LogInformation("成功獲取用戶銷售，用戶ID: {UserId}, 數量: {Count}", userId, sales.Count);
            return sales;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶銷售時發生錯誤，用戶ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// 計算平台費用 - 優化版本
    /// </summary>
    public async Task<decimal> CalculatePlatformFeeAsync(decimal amount)
    {
        _logger.LogDebug("開始計算平台費用，金額: {Amount}", amount);

        try
        {
            if (amount <= 0)
            {
                _logger.LogWarning("無效的金額: {Amount}", amount);
                return 0;
            }

            var fee = amount * PlatformFeeRate;
            var result = Math.Max(fee, MinimumPlatformFee);

            _logger.LogDebug("成功計算平台費用，金額: {Amount}, 費用: {Fee}", amount, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算平台費用時發生錯誤，金額: {Amount}", amount);
            throw;
        }
    }

    /// <summary>
    /// 清除玩家市場相關快取 - 新增方法
    /// </summary>
    public void ClearPlayerMarketRelatedCache(int userId)
    {
        try
        {
            _memoryCache.Remove(AllListingsCacheKey);
            _memoryCache.Remove(string.Format(UserListingsCacheKey, userId));
            _memoryCache.Remove(string.Format(UserOrdersCacheKey, userId));
            _memoryCache.Remove(string.Format(UserSalesCacheKey, userId));

            _logger.LogDebug("已清除玩家市場相關快取，用戶ID: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除玩家市場相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證創建掛牌請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateListingRequest(CreatePlayerMarketListingDto request)
    {
        var result = new ValidationResult();

        if (request.SellerId <= 0)
            result.AddError("賣家ID必須大於0");

        if (string.IsNullOrWhiteSpace(request.Title))
            result.AddError("標題不能為空");

        if (string.IsNullOrWhiteSpace(request.Description))
            result.AddError("描述不能為空");

        if (request.Price <= 0)
            result.AddError("價格必須大於0");

        if (request.Quantity <= 0)
            result.AddError("數量必須大於0");

        return result;
    }

    /// <summary>
    /// 驗證更新掛牌請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateUpdateListingRequest(UpdatePlayerMarketListingDto request)
    {
        var result = new ValidationResult();

        if (request.Title != null && string.IsNullOrWhiteSpace(request.Title))
            result.AddError("標題不能為空");

        if (request.Description != null && string.IsNullOrWhiteSpace(request.Description))
            result.AddError("描述不能為空");

        if (request.Price.HasValue && request.Price.Value <= 0)
            result.AddError("價格必須大於0");

        if (request.Quantity.HasValue && request.Quantity.Value <= 0)
            result.AddError("數量必須大於0");

        return result;
    }

    /// <summary>
    /// 驗證創建訂單請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateOrderRequest(CreatePlayerMarketOrderDto request)
    {
        var result = new ValidationResult();

        if (request.ListingId <= 0)
            result.AddError("掛牌ID必須大於0");

        if (request.BuyerId <= 0)
            result.AddError("買家ID必須大於0");

        if (request.Quantity <= 0)
            result.AddError("數量必須大於0");

        return result;
    }

    /// <summary>
    /// 更新掛牌欄位 - 新增方法
    /// </summary>
    private void UpdateListingFields(PlayerMarketListing listing, UpdatePlayerMarketListingDto updateDto)
    {
        if (updateDto.Title != null)
            listing.Title = updateDto.Title.Trim();

        if (updateDto.Description != null)
            listing.Description = updateDto.Description.Trim();

        if (updateDto.Price.HasValue)
            listing.Price = updateDto.Price.Value;

        if (updateDto.Quantity.HasValue)
        {
            var quantityChange = updateDto.Quantity.Value - listing.Quantity;
            listing.Quantity = updateDto.Quantity.Value;
            listing.AvailableQuantity = Math.Max(0, listing.AvailableQuantity + quantityChange);
        }

        if (updateDto.ImageUrl != null)
            listing.ImageUrl = updateDto.ImageUrl.Trim();

        if (updateDto.IsNegotiable.HasValue)
            listing.IsNegotiable = updateDto.IsNegotiable.Value;

        if (updateDto.ExpiresAt.HasValue)
            listing.ExpiresAt = updateDto.ExpiresAt.Value;
    }

    /// <summary>
    /// 更新掛牌數量 - 新增方法
    /// </summary>
    private void UpdateListingQuantity(PlayerMarketListing listing, int orderQuantity)
    {
        listing.AvailableQuantity -= orderQuantity;
        if (listing.AvailableQuantity == 0)
        {
            listing.Status = ListingStatus.Sold;
            listing.SoldAt = DateTime.UtcNow;
        }
        listing.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 恢復掛牌數量 - 新增方法
    /// </summary>
    private void RestoreListingQuantity(PlayerMarketListing listing, int orderQuantity)
    {
        listing.AvailableQuantity += orderQuantity;
        if (listing.Status == ListingStatus.Sold)
        {
            listing.Status = ListingStatus.Active;
            listing.SoldAt = null;
        }
        listing.UpdatedAt = DateTime.UtcNow;
    }

    #endregion
}