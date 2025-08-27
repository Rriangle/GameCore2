using GameCore.Domain.Entities;
using GameCore.Domain.Enums;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 優化版玩家市場服務測試 - 涵蓋新增的快取、驗證、事務管理和性能功能
/// </summary>
public class PlayerMarketServiceOptimizedTests
{
    private readonly GameCoreDbContext _context;
    private readonly PlayerMarketService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<PlayerMarketService>> _loggerMock;

    public PlayerMarketServiceOptimizedTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<PlayerMarketService>>();
        _service = new PlayerMarketService(_context, _memoryCache, _loggerMock.Object);

        // 種子測試資料
        SeedTestData();
    }

    private void SeedTestData()
    {
        // 建立測試用戶
        var seller = new User
        {
            Id = 1,
            Username = "seller",
            Email = "seller@example.com",
            PasswordHash = "hash",
            IsActive = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(seller);

        var buyer = new User
        {
            Id = 2,
            Username = "buyer",
            Email = "buyer@example.com",
            PasswordHash = "hash",
            IsActive = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(buyer);

        // 建立測試掛牌
        var listing = new PlayerMarketListing
        {
            Id = 1,
            SellerId = 1,
            Title = "Test Item",
            Description = "Test item description",
            Price = 100.00m,
            Quantity = 10,
            AvailableQuantity = 10,
            Status = ListingStatus.Active,
            ImageUrl = "test.jpg",
            IsNegotiable = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.PlayerMarketListings.Add(listing);

        // 建立測試訂單
        var order = new PlayerMarketOrder
        {
            Id = 1,
            ListingId = 1,
            BuyerId = 2,
            SellerId = 1,
            Quantity = 2,
            UnitPrice = 100.00m,
            TotalAmount = 200.00m,
            PlatformFee = 10.00m,
            SellerAmount = 190.00m,
            Status = PlayerMarketOrderStatus.Pending,
            BuyerNotes = "Test order",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.PlayerMarketOrders.Add(order);

        _context.SaveChanges();
    }

    #region 所有掛牌獲取測試

    [Fact]
    public async Task GetAllListingsAsync_ShouldReturnAllActiveListings()
    {
        // Act
        var result = await _service.GetAllListingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
        Assert.Equal(ListingStatus.Active, result.First().Status);
    }

    [Fact]
    public async Task GetAllListingsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllListingsAsync();
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllListingsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    [Fact]
    public async Task GetAllListingsAsync_ShouldOrderByCreatedAtDescending()
    {
        // Arrange - 添加更多掛牌
        var listing2 = new PlayerMarketListing
        {
            Id = 2,
            SellerId = 1,
            Title = "Test Item 2",
            Description = "Test item 2 description",
            Price = 150.00m,
            Quantity = 5,
            AvailableQuantity = 5,
            Status = ListingStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _context.PlayerMarketListings.Add(listing2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllListingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(2, result.First().Id); // 最新的應該在前面
        Assert.Equal(1, result.Last().Id);
    }

    #endregion

    #region 分類掛牌獲取測試

    [Fact]
    public async Task GetListingsByCategoryAsync_WithValidCategoryId_ShouldReturnListings()
    {
        // Act
        var result = await _service.GetListingsByCategoryAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task GetListingsByCategoryAsync_WithInvalidCategoryId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetListingsByCategoryAsync(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetListingsByCategoryAsync_WithZeroCategoryId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetListingsByCategoryAsync(0);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 用戶掛牌獲取測試

    [Fact]
    public async Task GetUserListingsAsync_WithValidUserId_ShouldReturnUserListings()
    {
        // Act
        var result = await _service.GetUserListingsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().SellerId);
        Assert.Equal("Test Item", result.First().Title);
    }

    [Fact]
    public async Task GetUserListingsAsync_WithInvalidUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserListingsAsync(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserListingsAsync_WithNonExistentUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserListingsAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserListingsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetUserListingsAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserListingsAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 掛牌ID獲取測試

    [Fact]
    public async Task GetListingByIdAsync_WithValidId_ShouldReturnListing()
    {
        // Act
        var result = await _service.GetListingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Item", result.Title);
        Assert.Equal(1, result.SellerId);
    }

    [Fact]
    public async Task GetListingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetListingByIdAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetListingByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetListingByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetListingByIdAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetListingByIdAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetListingByIdAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
    }

    #endregion

    #region 掛牌創建測試

    [Fact]
    public async Task CreateListingAsync_WithValidRequest_ShouldCreateListing()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "New Item",
            Description = "New item description",
            Price = 75.50m,
            Quantity = 25,
            ImageUrl = "new-item.jpg",
            IsNegotiable = false
        };

        // Act
        var result = await _service.CreateListingAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.SellerId);
        Assert.Equal("New Item", result.Title);
        Assert.Equal(75.50m, result.Price);
        Assert.Equal(25, result.Quantity);
        Assert.Equal(ListingStatus.Active, result.Status);
        Assert.False(result.IsNegotiable);
    }

    [Fact]
    public async Task CreateListingAsync_WithInvalidSellerId_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = -1,
            Title = "New Item",
            Description = "New item description",
            Price = 75.50m,
            Quantity = 25
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateListingAsync(request));
    }

    [Fact]
    public async Task CreateListingAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "",
            Description = "New item description",
            Price = 75.50m,
            Quantity = 25
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateListingAsync(request));
    }

    [Fact]
    public async Task CreateListingAsync_WithEmptyDescription_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "New Item",
            Description = "",
            Price = 75.50m,
            Quantity = 25
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateListingAsync(request));
    }

    [Fact]
    public async Task CreateListingAsync_WithInvalidPrice_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "New Item",
            Description = "New item description",
            Price = 0,
            Quantity = 25
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateListingAsync(request));
    }

    [Fact]
    public async Task CreateListingAsync_WithInvalidQuantity_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "New Item",
            Description = "New item description",
            Price = 75.50m,
            Quantity = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateListingAsync(request));
    }

    #endregion

    #region 掛牌更新測試

    [Fact]
    public async Task UpdateListingAsync_WithValidRequest_ShouldUpdateListing()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Title = "Updated Item",
            Price = 125.00m,
            Quantity = 75
        };

        // Act
        var result = await _service.UpdateListingAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Item", result.Title);
        Assert.Equal(125.00m, result.Price);
        Assert.Equal(75, result.Quantity);
        Assert.Equal(75, result.AvailableQuantity); // 可用數量應該同步更新
    }

    [Fact]
    public async Task UpdateListingAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Title = "Updated Item"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateListingAsync(-1, request));
    }

    [Fact]
    public async Task UpdateListingAsync_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Title = "Updated Item"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateListingAsync(999, request));
    }

    [Fact]
    public async Task UpdateListingAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Title = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateListingAsync(1, request));
    }

    [Fact]
    public async Task UpdateListingAsync_WithInvalidPrice_ShouldThrowException()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Price = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateListingAsync(1, request));
    }

    [Fact]
    public async Task UpdateListingAsync_WithInvalidQuantity_ShouldThrowException()
    {
        // Arrange
        var request = new UpdatePlayerMarketListingDto
        {
            Quantity = -5
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateListingAsync(1, request));
    }

    #endregion

    #region 掛牌刪除測試

    [Fact]
    public async Task DeleteListingAsync_WithValidId_ShouldDeleteListing()
    {
        // Act
        var result = await _service.DeleteListingAsync(1);

        // Assert
        Assert.True(result);
        
        var deletedListing = await _context.PlayerMarketListings.FindAsync(1);
        Assert.Equal(ListingStatus.Cancelled, deletedListing!.Status);
        Assert.NotNull(deletedListing.CancelledAt);
    }

    [Fact]
    public async Task DeleteListingAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteListingAsync(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteListingAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteListingAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 訂單創建測試

    [Fact]
    public async Task CreateOrderAsync_WithValidRequest_ShouldCreateOrder()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = 1,
            BuyerId = 2,
            Quantity = 3
        };

        // Act
        var result = await _service.CreateOrderAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ListingId);
        Assert.Equal(2, result.BuyerId);
        Assert.Equal(1, result.SellerId);
        Assert.Equal(3, result.Quantity);
        Assert.Equal(100.00m, result.UnitPrice);
        Assert.Equal(300.00m, result.TotalAmount);
        Assert.Equal(PlayerMarketOrderStatus.Pending, result.Status);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidListingId_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = -1,
            BuyerId = 2,
            Quantity = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidBuyerId_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = 1,
            BuyerId = 0,
            Quantity = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidQuantity_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = 1,
            BuyerId = 2,
            Quantity = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientQuantity_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = 1,
            BuyerId = 2,
            Quantity = 15 // 超過可用數量 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithBuyerAsSeller_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketOrderDto
        {
            ListingId = 1,
            BuyerId = 1, // 買家是賣家
            Quantity = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(request));
    }

    #endregion

    #region 訂單確認測試

    [Fact]
    public async Task ConfirmOrderAsync_WithValidOrder_ShouldConfirmOrder()
    {
        // Act
        var result = await _service.ConfirmOrderAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PlayerMarketOrderStatus.Confirmed, result.Status);
        Assert.NotNull(result.ConfirmedAt);
    }

    [Fact]
    public async Task ConfirmOrderAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.ConfirmOrderAsync(-1));
    }

    [Fact]
    public async Task ConfirmOrderAsync_WithNonExistentId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.ConfirmOrderAsync(999));
    }

    [Fact]
    public async Task ConfirmOrderAsync_WithNonPendingStatus_ShouldThrowException()
    {
        // Arrange - 先確認訂單
        await _service.ConfirmOrderAsync(1);

        // Act & Assert - 再次確認應該失敗
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ConfirmOrderAsync(1));
    }

    #endregion

    #region 訂單完成測試

    [Fact]
    public async Task CompleteOrderAsync_WithValidOrder_ShouldCompleteOrder()
    {
        // Arrange - 先確認訂單
        await _service.ConfirmOrderAsync(1);

        // Act
        var result = await _service.CompleteOrderAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PlayerMarketOrderStatus.Completed, result.Status);
        Assert.NotNull(result.CompletedAt);
    }

    [Fact]
    public async Task CompleteOrderAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CompleteOrderAsync(-1));
    }

    [Fact]
    public async Task CompleteOrderAsync_WithNonExistentId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CompleteOrderAsync(999));
    }

    [Fact]
    public async Task CompleteOrderAsync_WithNonConfirmedStatus_ShouldThrowException()
    {
        // Act & Assert - 直接完成未確認的訂單應該失敗
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteOrderAsync(1));
    }

    #endregion

    #region 訂單取消測試

    [Fact]
    public async Task CancelOrderAsync_WithValidOrder_ShouldCancelOrder()
    {
        // Act
        var result = await _service.CancelOrderAsync(1);

        // Assert
        Assert.True(result);
        
        var cancelledOrder = await _context.PlayerMarketOrders.FindAsync(1);
        Assert.Equal(PlayerMarketOrderStatus.Cancelled, cancelledOrder!.Status);
        Assert.NotNull(cancelledOrder.CancelledAt);
    }

    [Fact]
    public async Task CancelOrderAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.CancelOrderAsync(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CancelOrderAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.CancelOrderAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CancelOrderAsync_WithNonPendingStatus_ShouldReturnFalse()
    {
        // Arrange - 先確認訂單
        await _service.ConfirmOrderAsync(1);

        // Act - 取消已確認的訂單應該失敗
        var result = await _service.CancelOrderAsync(1);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 用戶訂單獲取測試

    [Fact]
    public async Task GetUserOrdersAsync_WithValidUserId_ShouldReturnUserOrders()
    {
        // Act
        var result = await _service.GetUserOrdersAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(2, result.First().BuyerId);
        Assert.Equal(1, result.First().ListingId);
    }

    [Fact]
    public async Task GetUserOrdersAsync_WithInvalidUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserOrdersAsync(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserOrdersAsync_WithNonExistentUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserOrdersAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserOrdersAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetUserOrdersAsync(2);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserOrdersAsync(2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 用戶銷售獲取測試

    [Fact]
    public async Task GetUserSalesAsync_WithValidUserId_ShouldReturnUserSales()
    {
        // Act
        var result = await _service.GetUserSalesAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().SellerId);
        Assert.Equal(1, result.First().ListingId);
    }

    [Fact]
    public async Task GetUserSalesAsync_WithInvalidUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserSalesAsync(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserSalesAsync_WithNonExistentUserId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetUserSalesAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserSalesAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetUserSalesAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserSalesAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 平台費用計算測試

    [Fact]
    public async Task CalculatePlatformFeeAsync_WithValidAmount_ShouldCalculateFee()
    {
        // Act
        var result = await _service.CalculatePlatformFeeAsync(100.00m);

        // Assert
        Assert.Equal(5.00m, result); // 5% of 100 = 5
    }

    [Fact]
    public async Task CalculatePlatformFeeAsync_WithSmallAmount_ShouldReturnMinimumFee()
    {
        // Act
        var result = await _service.CalculatePlatformFeeAsync(10.00m);

        // Assert
        Assert.Equal(1.00m, result); // 5% of 10 = 0.5, but minimum is 1
    }

    [Fact]
    public async Task CalculatePlatformFeeAsync_WithZeroAmount_ShouldReturnZero()
    {
        // Act
        var result = await _service.CalculatePlatformFeeAsync(0);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CalculatePlatformFeeAsync_WithNegativeAmount_ShouldReturnZero()
    {
        // Act
        var result = await _service.CalculatePlatformFeeAsync(-50.00m);

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearPlayerMarketRelatedCache_WithValidUserId_ShouldRemoveCacheEntries()
    {
        // Arrange - 先建立一些快取
        _memoryCache.Set("PlayerMarket_AllListings", new List<PlayerMarketListing>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("PlayerMarket_UserListings_1", new List<PlayerMarketListing>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("PlayerMarket_UserOrders_1", new List<PlayerMarketOrder>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("PlayerMarket_UserSales_1", new List<PlayerMarketOrder>(), TimeSpan.FromMinutes(5));

        // Act
        _service.ClearPlayerMarketRelatedCache(1);

        // Assert
        Assert.False(_memoryCache.TryGetValue("PlayerMarket_AllListings", out _));
        Assert.False(_memoryCache.TryGetValue("PlayerMarket_UserListings_1", out _));
        Assert.False(_memoryCache.TryGetValue("PlayerMarket_UserOrders_1", out _));
        Assert.False(_memoryCache.TryGetValue("PlayerMarket_UserSales_1", out _));
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task GetAllListingsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _service.GetAllListingsAsync());
    }

    [Fact]
    public async Task CreateListingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var request = new CreatePlayerMarketListingDto
        {
            SellerId = 1,
            Title = "New Item",
            Description = "New item description",
            Price = 75.50m,
            Quantity = 25
        };

        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _service.CreateListingAsync(request));
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetAllListingsAsync_WithLargeDataset_ShouldCompleteWithinReasonableTime()
    {
        // Arrange - 建立更多測試資料
        for (int i = 2; i <= 50; i++)
        {
            var listing = new PlayerMarketListing
            {
                Id = i,
                SellerId = 1,
                Title = $"Test Item {i}",
                Description = $"Test item {i} description",
                Price = 100.00m + i,
                Quantity = 10 + i,
                AvailableQuantity = 10 + i,
                Status = ListingStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            };
            _context.PlayerMarketListings.Add(listing);
        }
        await _context.SaveChangesAsync();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _service.GetAllListingsAsync();
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 2000); // 應該在2秒內完成
        Assert.NotNull(result);
        Assert.Equal(50, result.Count());
    }

    #endregion

    #region 邊界值測試

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetUserListingsAsync_WithBoundaryValues_ShouldReturnEmptyList(int userId)
    {
        // Act
        var result = await _service.GetUserListingsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetListingByIdAsync_WithBoundaryValues_ShouldReturnNull(int listingId)
    {
        // Act
        var result = await _service.GetListingByIdAsync(listingId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetUserOrdersAsync_WithBoundaryValues_ShouldReturnEmptyList(int userId)
    {
        // Act
        var result = await _service.GetUserOrdersAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _memoryCache.Dispose();
    }
}