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
/// 訂單服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class OrderService : IOrderService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<OrderService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 5;
    private const string UserOrdersCacheKey = "UserOrders_{0}";
    private const string OrderCacheKey = "Order_{0}";
    private const string OrdersByStatusCacheKey = "OrdersByStatus_{0}";
    private const decimal TaxRate = 0.1m; // 10% tax
    private const decimal FreeShippingThreshold = 100m;
    private const decimal ShippingCost = 10m;

    public OrderService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<OrderService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取用戶訂單 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶訂單，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<Order>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserOrdersCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order> cachedOrders))
            {
                _logger.LogDebug("從快取獲取用戶訂單，用戶ID: {UserId}, 數量: {Count}", userId, cachedOrders.Count());
                return cachedOrders;
            }

            // 從資料庫獲取
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
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
    /// 根據ID獲取訂單 - 優化版本（含快取）
    /// </summary>
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        _logger.LogInformation("開始根據ID獲取訂單，訂單ID: {OrderId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                return null;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(OrderCacheKey, id);
            if (_memoryCache.TryGetValue(cacheKey, out Order cachedOrder))
            {
                _logger.LogDebug("從快取獲取訂單，訂單ID: {OrderId}", id);
                return cachedOrder;
            }

            // 從資料庫獲取
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // 存入快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, order, cacheOptions);
            }

            _logger.LogInformation("成功獲取訂單，訂單ID: {OrderId}", id);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據ID獲取訂單時發生錯誤，訂單ID: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// 建立訂單 - 優化版本
    /// </summary>
    public async Task<Order> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        _logger.LogInformation("開始建立訂單，用戶ID: {UserId}", createOrderDto.UserId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateOrderRequest(createOrderDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("建立訂單請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 使用事務確保資料一致性
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 計算訂單總額
                var totalAmount = await CalculateOrderTotalAsync(createOrderDto);
                var taxAmount = totalAmount * TaxRate;
                var shippingAmount = totalAmount > FreeShippingThreshold ? 0 : ShippingCost;
                var finalAmount = totalAmount + taxAmount + shippingAmount;

                // 生成訂單編號
                var orderNumber = GenerateOrderNumber();

                var order = new Order
                {
                    OrderNumber = orderNumber,
                    UserId = createOrderDto.UserId,
                    TotalAmount = totalAmount,
                    TaxAmount = taxAmount,
                    ShippingAmount = shippingAmount,
                    FinalAmount = finalAmount,
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    ShippingAddress = createOrderDto.ShippingAddress?.Trim(),
                    ShippingCity = createOrderDto.ShippingCity?.Trim(),
                    ShippingPostalCode = createOrderDto.ShippingPostalCode?.Trim(),
                    ShippingCountry = createOrderDto.ShippingCountry?.Trim(),
                    ContactPhone = createOrderDto.ContactPhone?.Trim(),
                    Notes = createOrderDto.Notes?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 建立訂單項目
                await CreateOrderItemsAsync(order.Id, createOrderDto.OrderItems);

                // 更新產品庫存
                await UpdateProductStockAsync(createOrderDto.OrderItems);

                await transaction.CommitAsync();

                // 清除相關快取
                ClearOrderRelatedCache(createOrderDto.UserId);

                _logger.LogInformation("成功建立訂單，訂單ID: {OrderId}, 訂單編號: {OrderNumber}", order.Id, order.OrderNumber);
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
            _logger.LogError(ex, "建立訂單時發生錯誤，用戶ID: {UserId}", createOrderDto.UserId);
            throw;
        }
    }

    /// <summary>
    /// 更新訂單狀態 - 優化版本
    /// </summary>
    public async Task<Order> UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        _logger.LogInformation("開始更新訂單狀態，訂單ID: {OrderId}, 新狀態: {Status}", id, status);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                throw new ArgumentException("無效的訂單ID");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", id);
                throw new ArgumentException("訂單不存在");
            }

            // 驗證狀態轉換
            if (!IsValidStatusTransition(order.Status, status))
            {
                _logger.LogWarning("無效的狀態轉換，從 {CurrentStatus} 到 {NewStatus}", order.Status, status);
                throw new InvalidOperationException($"無法從 {order.Status} 轉換到 {status}");
            }

            // 更新訂單狀態
            UpdateOrderStatusFields(order, status);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearOrderRelatedCache(order.UserId);

            _logger.LogInformation("成功更新訂單狀態，訂單ID: {OrderId}, 新狀態: {Status}", id, status);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新訂單狀態時發生錯誤，訂單ID: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// 取消訂單 - 優化版本
    /// </summary>
    public async Task<bool> CancelOrderAsync(int id)
    {
        _logger.LogInformation("開始取消訂單，訂單ID: {OrderId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                return false;
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", id);
                return false;
            }

            if (!CanCancelOrder(order.Status))
            {
                _logger.LogWarning("訂單無法取消，當前狀態: {Status}", order.Status);
                return false;
            }

            // 使用事務確保資料一致性
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 更新訂單狀態
                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                // 恢復產品庫存
                await RestoreProductStockAsync(order.OrderItems);

                await transaction.CommitAsync();

                // 清除相關快取
                ClearOrderRelatedCache(order.UserId);

                _logger.LogInformation("成功取消訂單，訂單ID: {OrderId}", id);
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
            _logger.LogError(ex, "取消訂單時發生錯誤，訂單ID: {OrderId}", id);
            return false;
        }
    }

    /// <summary>
    /// 處理付款 - 優化版本
    /// </summary>
    public async Task<Order> ProcessPaymentAsync(int orderId, ProcessPaymentDto processPaymentDto)
    {
        _logger.LogInformation("開始處理付款，訂單ID: {OrderId}", orderId);

        try
        {
            // 輸入驗證
            if (orderId <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", orderId);
                throw new ArgumentException("無效的訂單ID");
            }

            var validationResult = ValidateProcessPaymentRequest(processPaymentDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("處理付款請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", orderId);
                throw new ArgumentException("訂單不存在");
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogWarning("訂單已付款，訂單ID: {OrderId}", orderId);
                throw new InvalidOperationException("訂單已付款");
            }

            // 使用事務確保資料一致性
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 建立付款交易記錄
                var paymentTransaction = new PaymentTransaction
                {
                    OrderId = orderId,
                    TransactionId = processPaymentDto.TransactionId?.Trim(),
                    Amount = order.FinalAmount,
                    PaymentMethod = processPaymentDto.PaymentMethod,
                    Status = PaymentStatus.Paid,
                    Description = processPaymentDto.Description?.Trim(),
                    PaymentDetails = processPaymentDto.PaymentDetails?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow
                };

                _context.PaymentTransactions.Add(paymentTransaction);

                // 更新訂單狀態
                order.PaymentStatus = PaymentStatus.Paid;
                order.Status = OrderStatus.Confirmed;
                order.PaidAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;

                await transaction.CommitAsync();

                // 清除相關快取
                ClearOrderRelatedCache(order.UserId);

                _logger.LogInformation("成功處理付款，訂單ID: {OrderId}", orderId);
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
            _logger.LogError(ex, "處理付款時發生錯誤，訂單ID: {OrderId}", orderId);
            throw;
        }
    }

    /// <summary>
    /// 確認訂單 - 優化版本
    /// </summary>
    public async Task<bool> ConfirmOrderAsync(int id)
    {
        _logger.LogInformation("開始確認訂單，訂單ID: {OrderId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", id);
                return false;
            }

            if (!CanConfirmOrder(order.Status, order.PaymentStatus))
            {
                _logger.LogWarning("訂單無法確認，當前狀態: {Status}, 付款狀態: {PaymentStatus}", order.Status, order.PaymentStatus);
                return false;
            }

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearOrderRelatedCache(order.UserId);

            _logger.LogInformation("成功確認訂單，訂單ID: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "確認訂單時發生錯誤，訂單ID: {OrderId}", id);
            return false;
        }
    }

    /// <summary>
    /// 發貨訂單 - 優化版本
    /// </summary>
    public async Task<bool> ShipOrderAsync(int id)
    {
        _logger.LogInformation("開始發貨訂單，訂單ID: {OrderId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", id);
                return false;
            }

            if (order.Status != OrderStatus.Confirmed)
            {
                _logger.LogWarning("訂單無法發貨，當前狀態: {Status}", order.Status);
                return false;
            }

            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearOrderRelatedCache(order.UserId);

            _logger.LogInformation("成功發貨訂單，訂單ID: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發貨訂單時發生錯誤，訂單ID: {OrderId}", id);
            return false;
        }
    }

    /// <summary>
    /// 交付訂單 - 優化版本
    /// </summary>
    public async Task<bool> DeliverOrderAsync(int id)
    {
        _logger.LogInformation("開始交付訂單，訂單ID: {OrderId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的訂單ID: {OrderId}", id);
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("訂單不存在，訂單ID: {OrderId}", id);
                return false;
            }

            if (order.Status != OrderStatus.Shipped)
            {
                _logger.LogWarning("訂單無法交付，當前狀態: {Status}", order.Status);
                return false;
            }

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearOrderRelatedCache(order.UserId);

            _logger.LogInformation("成功交付訂單，訂單ID: {OrderId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "交付訂單時發生錯誤，訂單ID: {OrderId}", id);
            return false;
        }
    }

    /// <summary>
    /// 根據狀態獲取訂單 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        _logger.LogInformation("開始根據狀態獲取訂單，狀態: {Status}", status);

        try
        {
            // 嘗試從快取獲取
            var cacheKey = string.Format(OrdersByStatusCacheKey, status);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order> cachedOrders))
            {
                _logger.LogDebug("從快取獲取訂單，狀態: {Status}, 數量: {Count}", status, cachedOrders.Count());
                return cachedOrders;
            }

            // 從資料庫獲取
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == status)
                .OrderBy(o => o.CreatedAt)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, orders, cacheOptions);

            _logger.LogInformation("成功獲取訂單，狀態: {Status}, 數量: {Count}", status, orders.Count);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據狀態獲取訂單時發生錯誤，狀態: {Status}", status);
            throw;
        }
    }

    /// <summary>
    /// 計算訂單總額 - 優化版本
    /// </summary>
    public async Task<decimal> CalculateOrderTotalAsync(CreateOrderDto createOrderDto)
    {
        _logger.LogDebug("開始計算訂單總額，用戶ID: {UserId}", createOrderDto.UserId);

        try
        {
            if (createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any())
            {
                _logger.LogWarning("訂單項目為空，用戶ID: {UserId}", createOrderDto.UserId);
                return 0;
            }

            // 並行獲取產品資訊以提高性能
            var productIds = createOrderDto.OrderItems.Select(item => item.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();

            var productDict = products.ToDictionary(p => p.Id, p => p);

            decimal total = 0;
            foreach (var item in createOrderDto.OrderItems)
            {
                if (productDict.TryGetValue(item.ProductId, out var product))
                {
                    total += product.Price * item.Quantity;
                }
                else
                {
                    _logger.LogWarning("產品不存在，產品ID: {ProductId}", item.ProductId);
                    throw new ArgumentException($"產品 {item.ProductId} 不存在");
                }
            }

            _logger.LogDebug("成功計算訂單總額，用戶ID: {UserId}, 總額: {Total}", createOrderDto.UserId, total);
            return total;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算訂單總額時發生錯誤，用戶ID: {UserId}", createOrderDto.UserId);
            throw;
        }
    }

    /// <summary>
    /// 清除訂單相關快取 - 新增方法
    /// </summary>
    public void ClearOrderRelatedCache(int userId)
    {
        try
        {
            _memoryCache.Remove(string.Format(UserOrdersCacheKey, userId));
            _memoryCache.Remove(string.Format(OrdersByStatusCacheKey, OrderStatus.Pending));
            _memoryCache.Remove(string.Format(OrdersByStatusCacheKey, OrderStatus.Confirmed));
            _memoryCache.Remove(string.Format(OrdersByStatusCacheKey, OrderStatus.Shipped));
            _memoryCache.Remove(string.Format(OrdersByStatusCacheKey, OrderStatus.Delivered));
            _memoryCache.Remove(string.Format(OrdersByStatusCacheKey, OrderStatus.Cancelled));

            _logger.LogDebug("已清除訂單相關快取，用戶ID: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除訂單相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證建立訂單請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateOrderRequest(CreateOrderDto request)
    {
        var result = new ValidationResult();

        if (request.UserId <= 0)
            result.AddError("用戶ID必須大於0");

        if (request.OrderItems == null || !request.OrderItems.Any())
            result.AddError("訂單項目不能為空");

        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            result.AddError("收貨地址不能為空");

        if (string.IsNullOrWhiteSpace(request.ShippingCity))
            result.AddError("收貨城市不能為空");

        if (string.IsNullOrWhiteSpace(request.ShippingCountry))
            result.AddError("收貨國家不能為空");

        if (string.IsNullOrWhiteSpace(request.ContactPhone))
            result.AddError("聯絡電話不能為空");

        return result;
    }

    /// <summary>
    /// 驗證處理付款請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateProcessPaymentRequest(ProcessPaymentDto request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            result.AddError("付款方式不能為空");

        if (string.IsNullOrWhiteSpace(request.TransactionId))
            result.AddError("交易ID不能為空");

        return result;
    }

    /// <summary>
    /// 建立訂單項目 - 新增方法
    /// </summary>
    private async Task CreateOrderItemsAsync(int orderId, List<CreateOrderItemDto> orderItems)
    {
        foreach (var itemDto in orderItems)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            if (product == null)
                throw new ArgumentException($"產品 {itemDto.ProductId} 不存在");

            if (product.StockQuantity < itemDto.Quantity)
                throw new ArgumentException($"產品 {product.Name} 庫存不足");

            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = itemDto.ProductId,
                ProductName = product.Name,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * itemDto.Quantity,
                ProductImageUrl = product.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.OrderItems.Add(orderItem);
        }
    }

    /// <summary>
    /// 更新產品庫存 - 新增方法
    /// </summary>
    private async Task UpdateProductStockAsync(List<CreateOrderItemDto> orderItems)
    {
        foreach (var item in orderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// 恢復產品庫存 - 新增方法
    /// </summary>
    private async Task RestoreProductStockAsync(IEnumerable<OrderItem> orderItems)
    {
        foreach (var item in orderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// 驗證狀態轉換 - 新增方法
    /// </summary>
    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.Confirmed => currentStatus == OrderStatus.Pending,
            OrderStatus.Shipped => currentStatus == OrderStatus.Confirmed,
            OrderStatus.Delivered => currentStatus == OrderStatus.Shipped,
            OrderStatus.Cancelled => currentStatus == OrderStatus.Pending || currentStatus == OrderStatus.Confirmed,
            _ => false
        };
    }

    /// <summary>
    /// 檢查是否可以取消訂單 - 新增方法
    /// </summary>
    private bool CanCancelOrder(OrderStatus status)
    {
        return status == OrderStatus.Pending || status == OrderStatus.Confirmed;
    }

    /// <summary>
    /// 檢查是否可以確認訂單 - 新增方法
    /// </summary>
    private bool CanConfirmOrder(OrderStatus status, PaymentStatus paymentStatus)
    {
        return status == OrderStatus.Pending && paymentStatus == PaymentStatus.Paid;
    }

    /// <summary>
    /// 更新訂單狀態欄位 - 新增方法
    /// </summary>
    private void UpdateOrderStatusFields(Order order, OrderStatus status)
    {
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        switch (status)
        {
            case OrderStatus.Confirmed:
                order.UpdatedAt = DateTime.UtcNow;
                break;
            case OrderStatus.Shipped:
                order.ShippedAt = DateTime.UtcNow;
                break;
            case OrderStatus.Delivered:
                order.DeliveredAt = DateTime.UtcNow;
                break;
            case OrderStatus.Cancelled:
                order.CancelledAt = DateTime.UtcNow;
                break;
        }
    }

    /// <summary>
    /// 生成訂單編號 - 優化版本
    /// </summary>
    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    #endregion
}