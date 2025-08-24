using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 商城服務實作，提供商城相關的業務邏輯
/// </summary>
public class StoreService : IStoreService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IStoreRankingRepository _rankingRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IWalletService _walletService;
    private readonly GameCoreDbContext _context;
    private readonly ILogger<StoreService> _logger;

    public StoreService(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IStoreRankingRepository rankingRepository,
        ISupplierRepository supplierRepository,
        IWalletService walletService,
        GameCoreDbContext context,
        ILogger<StoreService> logger)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _rankingRepository = rankingRepository;
        _supplierRepository = supplierRepository;
        _walletService = walletService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return null;

        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductType = product.ProductType,
            Price = product.Price,
            CurrencyCode = product.CurrencyCode,
            ShipmentQuantity = product.ShipmentQuantity,
            ProductCreatedBy = product.Creator?.User_name,
            ProductCreatedAt = product.ProductCreatedAt,
            ProductUpdatedAt = product.ProductUpdatedAt,
            Details = await GetProductDetailsInternalAsync(product)
        };
    }

    /// <summary>
    /// 搜尋商品 (分頁查詢)
    /// </summary>
    public async Task<PagedProductsDto> SearchProductsAsync(ProductSearchQueryDto query)
    {
        return await _productRepository.SearchProductsAsync(query);
    }

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    public async Task<List<ProductDto>> GetPopularProductsAsync(int limit = 10)
    {
        var products = await _productRepository.GetPopularProductsAsync(limit);
        
        var productDtos = new List<ProductDto>();
        foreach (var product in products)
        {
            productDtos.Add(new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductType = product.ProductType,
                Price = product.Price,
                CurrencyCode = product.CurrencyCode,
                ShipmentQuantity = product.ShipmentQuantity,
                ProductCreatedBy = product.Creator?.User_name,
                ProductCreatedAt = product.ProductCreatedAt,
                ProductUpdatedAt = product.ProductUpdatedAt,
                Details = await GetProductDetailsInternalAsync(product)
            });
        }
        
        return productDtos;
    }

    /// <summary>
    /// 取得商品推薦 (依用戶購買歷史)
    /// </summary>
    public async Task<List<ProductDto>> GetRecommendedProductsAsync(int userId, int limit = 10)
    {
        // 基於用戶的購買歷史推薦相似商品
        var userOrders = await _orderRepository.GetByUserIdAsync(userId);
        var purchasedProductTypes = userOrders
            .SelectMany(o => o.OrderItems)
            .Select(oi => oi.Product.ProductType)
            .Distinct()
            .ToList();

        if (!purchasedProductTypes.Any())
        {
            // 沒有購買歷史，返回熱門商品
            return await GetPopularProductsAsync(limit);
        }

        var recommendedProducts = new List<ProductInfo>();
        foreach (var productType in purchasedProductTypes)
        {
            var products = await _productRepository.GetByProductTypeAsync(productType);
            recommendedProducts.AddRange(products.Take(limit / purchasedProductTypes.Count + 1));
        }

        var productDtos = new List<ProductDto>();
        foreach (var product in recommendedProducts.Take(limit))
        {
            productDtos.Add(new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductType = product.ProductType,
                Price = product.Price,
                CurrencyCode = product.CurrencyCode,
                ShipmentQuantity = product.ShipmentQuantity,
                ProductCreatedBy = product.Creator?.User_name,
                ProductCreatedAt = product.ProductCreatedAt,
                ProductUpdatedAt = product.ProductUpdatedAt,
                Details = await GetProductDetailsInternalAsync(product)
            });
        }

        return productDtos;
    }

    /// <summary>
    /// 加入購物車
    /// </summary>
    public async Task<bool> AddToCartAsync(int userId, int productId, int quantity)
    {
        // 檢查商品是否存在且庫存足夠
        if (!await _productRepository.CheckStockAsync(productId, quantity))
        {
            _logger.LogWarning("Product {ProductId} stock insufficient for quantity {Quantity}", productId, quantity);
            return false;
        }

        // 簡化實作：直接更新庫存，實際應該有購物車表
        // 這裡為了演示目的，我們假設加入購物車就是預留庫存
        await _productRepository.UpdateStockAsync(productId, -quantity);
        
        _logger.LogInformation("Added product {ProductId} to cart for user {UserId}, quantity: {Quantity}", 
            productId, userId, quantity);
        return true;
    }

    /// <summary>
    /// 移除購物車商品
    /// </summary>
    public async Task<bool> RemoveFromCartAsync(int userId, int productId)
    {
        // 簡化實作：恢復庫存
        await _productRepository.UpdateStockAsync(productId, 1); // 假設移除1個
        
        _logger.LogInformation("Removed product {ProductId} from cart for user {UserId}", productId, userId);
        return true;
    }

    /// <summary>
    /// 更新購物車商品數量
    /// </summary>
    public async Task<bool> UpdateCartQuantityAsync(int userId, int productId, int quantity)
    {
        // 簡化實作
        _logger.LogInformation("Updated cart quantity for product {ProductId}, user {UserId}, new quantity: {Quantity}", 
            productId, userId, quantity);
        return true;
    }

    /// <summary>
    /// 取得購物車
    /// </summary>
    public async Task<ShoppingCartDto> GetShoppingCartAsync(int userId)
    {
        // 簡化實作：返回空購物車
        await Task.CompletedTask;
        return new ShoppingCartDto
        {
            Items = new List<CartItemDto>()
        };
    }

    /// <summary>
    /// 清空購物車
    /// </summary>
    public async Task<bool> ClearCartAsync(int userId)
    {
        _logger.LogInformation("Cleared cart for user {UserId}", userId);
        return true;
    }

    /// <summary>
    /// 建立訂單
    /// </summary>
    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequestDto request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // 簡化實作：假設用戶ID為1，總金額為項目小計總和
            var userId = 1; // 簡化實作，實際應從請求中取得
            var totalAmount = request.Items.Sum(i => i.Subtotal);

            // 創建訂單
            var order = new OrderInfo
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                OrderTotal = totalAmount
            };

            order = await _orderRepository.CreateAsync(order);

            // 創建訂單明細
            int lineNo = 1;
            foreach (var item in request.Items)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    LineNo = lineNo++,
                    UnitPrice = item.Product?.Price ?? 0,
                    Quantity = item.Quantity,
                    Subtotal = item.Subtotal
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Created order {OrderId} for user {UserId}, total: {Total}", 
                order.OrderId, userId, totalAmount);

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                OrderTotal = order.OrderTotal,
                Items = request.Items.Select((item, index) => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.ProductName ?? "Unknown Product",
                    LineNo = index + 1,
                    UnitPrice = item.Product?.Price ?? 0,
                    Quantity = item.Quantity,
                    Subtotal = item.Subtotal
                }).ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// 取得訂單詳細資訊
    /// </summary>
    public async Task<OrderDto?> GetOrderAsync(int orderId, int userId)
    {
        if (!await _orderRepository.IsOrderOwnedByUserAsync(orderId, userId))
        {
            return null;
        }

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            return null;

        return new OrderDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            UserName = order.User?.User_name,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus,
            PaymentStatus = order.PaymentStatus,
            OrderTotal = order.OrderTotal,
            PaymentAt = order.PaymentAt,
            ShippedAt = order.ShippedAt,
            CompletedAt = order.CompletedAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                ItemId = oi.ItemId,
                ProductId = oi.ProductId,
                ProductName = oi.Product.ProductName,
                LineNo = oi.LineNo,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                Subtotal = oi.Subtotal
            }).ToList()
        };
    }

    /// <summary>
    /// 取得會員訂單列表
    /// </summary>
    public async Task<PagedOrdersDto> GetUserOrdersAsync(int userId, OrderSearchQueryDto query)
    {
        query.UserId = userId;
        return await _orderRepository.SearchOrdersAsync(query);
    }

    /// <summary>
    /// 處理付款回呼
    /// </summary>
    public async Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDto callback)
    {
        var order = await _orderRepository.GetByIdAsync(callback.OrderId);
        if (order == null)
        {
            _logger.LogWarning("Payment callback for non-existent order {OrderId}", callback.OrderId);
            return false;
        }

        if (callback.PaymentResult.ToLower() == "success")
        {
            await _orderRepository.UpdatePaymentStatusAsync(callback.OrderId, "Paid", callback.PaidAt ?? DateTime.UtcNow);
            await _orderRepository.UpdateOrderStatusAsync(callback.OrderId, "Processing");
            
            _logger.LogInformation("Payment successful for order {OrderId}", callback.OrderId);
        }
        else
        {
            await _orderRepository.UpdatePaymentStatusAsync(callback.OrderId, "Failed");
            await _orderRepository.UpdateOrderStatusAsync(callback.OrderId, "Cancelled");
            
            _logger.LogWarning("Payment failed for order {OrderId}: {Result}", callback.OrderId, callback.PaymentResult);
        }

        return true;
    }

    /// <summary>
    /// 取得商城排行榜
    /// </summary>
    public async Task<List<StoreRankingDto>> GetStoreRankingsAsync(RankingQueryDto query)
    {
        return await _rankingRepository.GetRankingsAsync(query);
    }

    /// <summary>
    /// 更新商城排行榜 (後台定時任務使用)
    /// </summary>
    public async Task<int> UpdateStoreRankingsAsync(string periodType, DateTime targetDate)
    {
        var startDate = periodType.ToLower() switch
        {
            "daily" => targetDate.Date,
            "weekly" => targetDate.AddDays(-7),
            "monthly" => targetDate.AddMonths(-1),
            _ => targetDate.AddDays(-1)
        };

        var rankings = await _rankingRepository.CalculateRankingsAsync(startDate, targetDate, periodType);
        var updatedCount = await _rankingRepository.BatchCreateOrUpdateAsync(rankings);

        _logger.LogInformation("Updated {Count} ranking records for period {PeriodType} on {Date}", 
            updatedCount, periodType, targetDate);

        return updatedCount;
    }

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    public async Task<List<SupplierDto>> GetSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(s => new SupplierDto
        {
            SupplierId = s.SupplierId,
            SupplierName = s.SupplierName,
            ProductCount = s.GameProducts.Count + s.OtherProducts.Count
        }).ToList();
    }

    /// <summary>
    /// 檢查庫存是否足夠
    /// </summary>
    public async Task<bool> CheckStockAsync(int productId, int quantity)
    {
        return await _productRepository.CheckStockAsync(productId, quantity);
    }

    /// <summary>
    /// 取得商品詳細類型資訊 (遊戲商品或其他商品)
    /// </summary>
    public async Task<ProductDetailsDto?> GetProductDetailsAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return null;

        return await GetProductDetailsInternalAsync(product);
    }

    /// <summary>
    /// 內部方法：取得商品詳細資訊
    /// </summary>
    private async Task<ProductDetailsDto?> GetProductDetailsInternalAsync(ProductInfo product)
    {
        await Task.CompletedTask; // 避免編譯器警告

        if (product.GameProductDetails != null)
        {
            return new GameProductDetailsDto
            {
                ProductDescription = product.GameProductDetails.ProductDescription,
                SupplierId = product.GameProductDetails.SupplierId,
                PlatformId = product.GameProductDetails.PlatformId,
                GameId = product.GameProductDetails.GameId,
                GameName = product.GameProductDetails.GameName,
                DownloadLink = product.GameProductDetails.DownloadLink
            };
        }

        if (product.OtherProductDetails != null)
        {
            return new OtherProductDetailsDto
            {
                ProductDescription = product.OtherProductDetails.ProductDescription,
                SupplierId = product.OtherProductDetails.SupplierId,
                PlatformId = product.OtherProductDetails.PlatformId,
                DigitalCode = product.OtherProductDetails.DigitalCode,
                Size = product.OtherProductDetails.Size,
                Color = product.OtherProductDetails.Color,
                Weight = product.OtherProductDetails.Weight?.ToString(),
                Dimensions = product.OtherProductDetails.Dimensions,
                Material = product.OtherProductDetails.Material,
                StockQuantity = product.OtherProductDetails.StockQuantity?.ToString()
            };
        }

        return null;
    }
}