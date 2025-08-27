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
/// 優化版訂單服務測試 - 涵蓋新增的快取、驗證、事務管理和性能功能
/// </summary>
public class OrderServiceOptimizedTests
{
    private readonly GameCoreDbContext _context;
    private readonly OrderService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<OrderService>> _loggerMock;

    public OrderServiceOptimizedTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<OrderService>>();
        _service = new OrderService(_context, _memoryCache, _loggerMock.Object);

        // 種子測試資料
        SeedTestData();
    }

    private void SeedTestData()
    {
        // 建立測試用戶
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            IsActive = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(user);

        // 建立測試產品分類
        var category = new ProductCategory
        {
            Id = 1,
            Name = "Test Category",
            Description = "Test category description",
            IsActive = true,
            SortOrder = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.ProductCategories.Add(category);

        // 建立測試產品
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test product description",
            Price = 100.00m,
            StockQuantity = 50,
            CategoryId = 1,
            IsActive = true,
            IsOfficialStore = true,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };
        _context.Products.Add(product);

        // 建立測試訂單
        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD-20240101-12345678",
            UserId = 1,
            TotalAmount = 100.00m,
            TaxAmount = 10.00m,
            ShippingAmount = 0,
            FinalAmount = 110.00m,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ShippingAddress = "123 Test St",
            ShippingCity = "Test City",
            ShippingPostalCode = "12345",
            ShippingCountry = "Test Country",
            ContactPhone = "123-456-7890",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Orders.Add(order);

        // 建立測試訂單項目
        var orderItem = new OrderItem
        {
            Id = 1,
            OrderId = 1,
            ProductId = 1,
            ProductName = "Test Product",
            Quantity = 1,
            UnitPrice = 100.00m,
            TotalPrice = 100.00m,
            ProductImageUrl = "test.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.OrderItems.Add(orderItem);

        _context.SaveChanges();
    }

    #region 用戶訂單獲取測試

    [Fact]
    public async Task GetUserOrdersAsync_WithValidUserId_ShouldReturnOrders()
    {
        // Act
        var result = await _service.GetUserOrdersAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
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
        var result1 = await _service.GetUserOrdersAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserOrdersAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 訂單ID獲取測試

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ShouldReturnOrder()
    {
        // Act
        var result = await _service.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("ORD-20240101-12345678", result.OrderNumber);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetOrderByIdAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetOrderByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetOrderByIdAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
    }

    #endregion

    #region 訂單建立測試

    [Fact]
    public async Task CreateOrderAsync_WithValidRequest_ShouldCreateOrder()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            },
            ShippingAddress = "456 New St",
            ShippingCity = "New City",
            ShippingPostalCode = "54321",
            ShippingCountry = "New Country",
            ContactPhone = "987-654-3210"
        };

        // Act
        var result = await _service.CreateOrderAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Equal(PaymentStatus.Pending, result.PaymentStatus);
        Assert.True(result.FinalAmount > 0);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = -1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 1 }
            },
            ShippingAddress = "Test Address",
            ShippingCity = "Test City",
            ShippingPostalCode = "12345",
            ShippingCountry = "Test Country",
            ContactPhone = "123-456-7890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithEmptyOrderItems_ShouldThrowException()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>(),
            ShippingAddress = "Test Address",
            ShippingCity = "Test City",
            ShippingPostalCode = "12345",
            ShippingCountry = "Test Country",
            ContactPhone = "123-456-7890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_WithMissingShippingInfo_ShouldThrowException()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 1 }
            },
            ShippingAddress = "",
            ShippingCity = "Test City",
            ShippingPostalCode = "12345",
            ShippingCountry = "Test Country",
            ContactPhone = "123-456-7890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    #endregion

    #region 訂單狀態更新測試

    [Fact]
    public async Task UpdateOrderStatusAsync_WithValidTransition_ShouldUpdateStatus()
    {
        // Act
        var result = await _service.UpdateOrderStatusAsync(1, OrderStatus.Confirmed);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidTransition_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateOrderStatusAsync(1, OrderStatus.Delivered));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateOrderStatusAsync(-1, OrderStatus.Confirmed));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithNonExistentOrder_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateOrderStatusAsync(999, OrderStatus.Confirmed));
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
        
        var cancelledOrder = await _context.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Cancelled, cancelledOrder!.Status);
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
    public async Task CancelOrderAsync_WithNonExistentOrder_ShouldReturnFalse()
    {
        // Act
        var result = await _service.CancelOrderAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 付款處理測試

    [Fact]
    public async Task ProcessPaymentAsync_WithValidRequest_ShouldProcessPayment()
    {
        // Arrange
        var request = new ProcessPaymentDto
        {
            TransactionId = "TXN123456",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };

        // Act
        var result = await _service.ProcessPaymentAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Paid, result.PaymentStatus);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithInvalidOrderId_ShouldThrowException()
    {
        // Arrange
        var request = new ProcessPaymentDto
        {
            TransactionId = "TXN123456",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessPaymentAsync(-1, request));
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithMissingTransactionId_ShouldThrowException()
    {
        // Arrange
        var request = new ProcessPaymentDto
        {
            TransactionId = "",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessPaymentAsync(1, request));
    }

    #endregion

    #region 訂單確認測試

    [Fact]
    public async Task ConfirmOrderAsync_WithPaidOrder_ShouldConfirmOrder()
    {
        // Arrange - 先處理付款
        var paymentRequest = new ProcessPaymentDto
        {
            TransactionId = "TXN123456",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };
        await _service.ProcessPaymentAsync(1, paymentRequest);

        // Act
        var result = await _service.ConfirmOrderAsync(1);

        // Assert
        Assert.True(result);
        
        var confirmedOrder = await _context.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Confirmed, confirmedOrder!.Status);
    }

    [Fact]
    public async Task ConfirmOrderAsync_WithUnpaidOrder_ShouldReturnFalse()
    {
        // Act
        var result = await _service.ConfirmOrderAsync(1);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 訂單發貨測試

    [Fact]
    public async Task ShipOrderAsync_WithConfirmedOrder_ShouldShipOrder()
    {
        // Arrange - 先確認訂單
        var paymentRequest = new ProcessPaymentDto
        {
            TransactionId = "TXN123456",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };
        await _service.ProcessPaymentAsync(1, paymentRequest);
        await _service.ConfirmOrderAsync(1);

        // Act
        var result = await _service.ShipOrderAsync(1);

        // Assert
        Assert.True(result);
        
        var shippedOrder = await _context.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Shipped, shippedOrder!.Status);
    }

    [Fact]
    public async Task ShipOrderAsync_WithUnconfirmedOrder_ShouldReturnFalse()
    {
        // Act
        var result = await _service.ShipOrderAsync(1);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 訂單交付測試

    [Fact]
    public async Task DeliverOrderAsync_WithShippedOrder_ShouldDeliverOrder()
    {
        // Arrange - 先發貨訂單
        var paymentRequest = new ProcessPaymentDto
        {
            TransactionId = "TXN123456",
            PaymentMethod = "CreditCard",
            Description = "Test payment",
            PaymentDetails = "Test details"
        };
        await _service.ProcessPaymentAsync(1, paymentRequest);
        await _service.ConfirmOrderAsync(1);
        await _service.ShipOrderAsync(1);

        // Act
        var result = await _service.DeliverOrderAsync(1);

        // Assert
        Assert.True(result);
        
        var deliveredOrder = await _context.Orders.FindAsync(1);
        Assert.Equal(OrderStatus.Delivered, deliveredOrder!.Status);
    }

    [Fact]
    public async Task DeliverOrderAsync_WithUnshippedOrder_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeliverOrderAsync(1);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 狀態訂單獲取測試

    [Fact]
    public async Task GetOrdersByStatusAsync_WithValidStatus_ShouldReturnOrders()
    {
        // Act
        var result = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(OrderStatus.Pending, result.First().Status);
    }

    [Fact]
    public async Task GetOrdersByStatusAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 訂單總額計算測試

    [Fact]
    public async Task CalculateOrderTotalAsync_WithValidItems_ShouldCalculateTotal()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };

        // Act
        var result = await _service.CalculateOrderTotalAsync(request);

        // Assert
        Assert.Equal(200.00m, result); // 100 * 2
    }

    [Fact]
    public async Task CalculateOrderTotalAsync_WithEmptyItems_ShouldReturnZero()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>()
        };

        // Act
        var result = await _service.CalculateOrderTotalAsync(request);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CalculateOrderTotalAsync_WithNonExistentProduct_ShouldThrowException()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 999, Quantity = 1 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculateOrderTotalAsync(request));
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearOrderRelatedCache_WithValidUserId_ShouldRemoveCacheEntries()
    {
        // Arrange - 先建立一些快取
        _memoryCache.Set("UserOrders_1", new List<Order>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("OrdersByStatus_Pending", new List<Order>(), TimeSpan.FromMinutes(5));

        // Act
        _service.ClearOrderRelatedCache(1);

        // Assert
        Assert.False(_memoryCache.TryGetValue("UserOrders_1", out _));
        Assert.False(_memoryCache.TryGetValue("OrdersByStatus_Pending", out _));
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task GetUserOrdersAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            _service.GetUserOrdersAsync(1));
    }

    [Fact]
    public async Task CreateOrderAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = new List<CreateOrderItemDto>
            {
                new() { ProductId = 1, Quantity = 1 }
            },
            ShippingAddress = "Test Address",
            ShippingCity = "Test City",
            ShippingPostalCode = "12345",
            ShippingCountry = "Test Country",
            ContactPhone = "123-456-7890"
        };

        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            _service.CreateOrderAsync(request));
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task CalculateOrderTotalAsync_WithMultipleItems_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var request = new CreateOrderDto
        {
            UserId = 1,
            OrderItems = Enumerable.Range(1, 10).Select(i => new CreateOrderItemDto
            {
                ProductId = 1,
                Quantity = i
            }).ToList()
        };

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _service.CalculateOrderTotalAsync(request);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
        Assert.True(result > 0);
    }

    #endregion

    #region 邊界值測試

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

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetOrderByIdAsync_WithBoundaryValues_ShouldReturnNull(int orderId)
    {
        // Act
        var result = await _service.GetOrderByIdAsync(orderId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _memoryCache.Dispose();
    }
}