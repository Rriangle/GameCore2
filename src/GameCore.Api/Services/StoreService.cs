using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace GameCore.Api.Services;

/// <summary>
/// 官方商店服務
/// 處理商店相關業務邏輯
/// </summary>
public class StoreService : IStoreService
{
    private readonly IProductInfoRepository _productRepository;
    private readonly IOrderInfoRepository _orderRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ISupplierRepository _supplierRepository;
    // 效能優化：注入記憶體快取服務
    // 目的：為熱點查詢結果提供快取，減少重複資料庫查詢
    // 前後差異：原本每次都查詢資料庫 → 快取命中時直接返回快取結果
    // 風險與回滾：需注意快取一致性；問題時可移除快取邏輯或調整 TTL
    private readonly IMemoryCache _memoryCache;

    public StoreService(
        IProductInfoRepository productRepository,
        IOrderInfoRepository orderRepository,
        IUserWalletRepository walletRepository,
        ISupplierRepository supplierRepository,
        IMemoryCache memoryCache)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _walletRepository = walletRepository;
        _supplierRepository = supplierRepository;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// 取得商品列表
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>商品列表</returns>
    public async Task<GameCore.Shared.DTOs.PagedResult<ProductDto>> GetProductsAsync(
        string? category = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20)
    {
        var products = await _productRepository.GetProductsAsync(category, searchTerm, page, pageSize);
        var total = await _productRepository.GetProductCountAsync(category, searchTerm);

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductID,
            ProductName = p.ProductName,
            Category = p.ProductType,
            Price = p.Price,
            Description = p.Description ?? "",
            StockQuantity = p.ShipmentQuantity,
            SupplierId = p.UserID, // 使用 UserID 作為供應商ID
            IsActive = true, // 暫時設為true
            CreatedAt = p.ProductCreatedAt
        }).ToList();

        return new GameCore.Shared.DTOs.PagedResult<ProductDto>
        {
            Data = productDtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳細資訊</returns>
    public async Task<ProductDetailDto?> GetProductDetailAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return null;

        var supplier = product.SupplierID.HasValue ? await _supplierRepository.GetByIdAsync(product.SupplierID.Value) : null;
        
        return new ProductDetailDto
        {
            ProductId = product.ProductID,
            ProductName = product.ProductName,
            Category = product.ProductType,
            Price = product.Price,
            Description = product.Description ?? "",
            StockQuantity = product.ShipmentQuantity,
            SupplierId = product.UserID,
            SupplierName = supplier?.SupplierName ?? "未知供應商",
            IsActive = true,
            CreatedAt = product.ProductCreatedAt,
            UpdatedAt = product.ProductCreatedAt
        };
    }

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="request">訂單請求</param>
    /// <returns>訂單結果</returns>
    public async Task<ServiceResult<OrderDto>> CreateOrderAsync(int userId, CreateOrderRequest request)
    {
        if (request.Items == null || !request.Items.Any())
            return ServiceResult<OrderDto>.CreateFailure("訂單項目不能為空");

        // 驗證商品庫存
        var orderItems = new List<OrderItemDto>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                return ServiceResult<OrderDto>.CreateFailure($"商品 {item.ProductId} 不存在");

            // 暫時移除下架檢查
            // if (!true) // 暫時設為true
            //     return ServiceResult<OrderDto>.CreateFailure($"商品 {product.ProductName} 已下架");

            if (product.ShipmentQuantity < item.Quantity)
                return ServiceResult<OrderDto>.CreateFailure($"商品 {product.ProductName} 庫存不足");

            var itemTotal = product.Price * item.Quantity;
            totalAmount += itemTotal;

            orderItems.Add(new OrderItemDto
            {
                ProductId = product.ProductID,
                ProductName = product.ProductName,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                TotalPrice = itemTotal
            });
        }

        // 檢查使用者點數是否足夠
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            return ServiceResult<OrderDto>.CreateFailure("使用者錢包不存在");

        if (wallet.User_Point < (int)totalAmount)
            return ServiceResult<OrderDto>.CreateFailure("點數不足");

        // 扣除點數
        var deductResult = await _walletRepository.DeductPointsAsync(userId, (int)totalAmount, $"購買商品訂單");
        if (!deductResult)
            return ServiceResult<OrderDto>.CreateFailure("扣除點數失敗");

        // 建立訂單
        var order = new OrderInfo
        {
            UserID = userId,
            OrderDate = DateTime.UtcNow,
            OrderTotal = totalAmount,
            OrderStatus = "ToShip",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var orderId = await _orderRepository.AddAsync(order);

        // 建立訂單項目
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            var orderItem = new OrderItem
            {
                OrderID = orderId,
                ProductID = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product?.Price ?? 0,
                Subtotal = (product?.Price ?? 0) * item.Quantity
            };

            await _orderRepository.AddOrderItemAsync(orderItem);

            // 更新庫存
            if (product != null)
            {
                product.ShipmentQuantity -= orderItem.Quantity;
                product.ProductUpdatedAt = DateTime.UtcNow;
                await _productRepository.UpdateAsync(product);
            }
        }

        var orderDto = new OrderDto
        {
            OrderId = orderId,
            UserId = userId,
            OrderDate = order.OrderDate,
            TotalAmount = totalAmount,
            Status = order.OrderStatus,
            Items = orderItems
        };

        return ServiceResult<OrderDto>.CreateSuccess(orderDto);
    }

    /// <summary>
    /// 取得使用者訂單列表
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    public async Task<GameCore.Shared.DTOs.PagedResult<OrderDto>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, page, pageSize);
        var total = await _orderRepository.GetCountByUserIdAsync(userId);

        var orderDtos = new List<OrderDto>();
        foreach (var order in orders)
        {
            var items = await _orderRepository.GetOrderItemsAsync(order.OrderID);
            var itemDtos = items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductID,
                ProductName = "", // 可以從商品表取得
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList();

            orderDtos.Add(new OrderDto
            {
                OrderId = order.OrderID,
                UserId = order.UserID,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderTotal,
                Status = order.OrderStatus,
                Items = itemDtos
            });
        }

        return new GameCore.Shared.DTOs.PagedResult<OrderDto>
        {
            Data = orderDtos,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    /// <summary>
    /// 取得商品類別列表
    /// </summary>
    /// <returns>商品類別列表</returns>
    public async Task<List<string>> GetProductCategoriesAsync()
    {
        // 效能優化：記憶體快取（1小時）
        // 目的：類別變動頻率極低，啟用較長快取時間
        // 前後差異：原本每次都查詢資料庫 → 快取命中時直接返回快取結果
        // 風險與回滾：若後台調整分類未即時，請縮短 TTL 或在修改後清快取
        const string cacheKey = "product_categories";
        if (_memoryCache.TryGetValue(cacheKey, out List<string>? cachedCategories))
        {
            return cachedCategories ?? new List<string>();
        }

        var categories = await _productRepository.GetCategoriesAsync();
        
        // 設定快取選項：1小時過期，滑動過期
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        
        _memoryCache.Set(cacheKey, categories, cacheOptions);
        return categories;
    }

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>熱門商品列表</returns>
    public async Task<List<ProductDto>> GetPopularProductsAsync(int top = 10)
    {
        // 效能優化：記憶體快取（15分鐘）
        // 目的：熱門商品榜單每 15 分鐘更新即可
        // 前後差異：原本每次都查詢資料庫 → 快取命中時直接返回快取結果
        // 風險與回滾：若有促銷/熱度快速變化，請縮短 TTL
        var cacheKey = $"popular_products_{top}";
        if (_memoryCache.TryGetValue(cacheKey, out List<ProductDto>? cachedProducts))
        {
            return cachedProducts ?? new List<ProductDto>();
        }

        var products = await _productRepository.GetPopularProductsAsync(top);
        
        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductID,
            ProductName = p.Product_Name,
            Category = p.Category,
            Price = p.Price,
            Description = p.Description ?? string.Empty,
            StockQuantity = p.Stock_Quantity,
            SupplierId = p.SupplierID ?? 0,
            IsActive = p.Is_Active,
            CreatedAt = p.Created_At
        }).ToList();

        // 設定快取選項：15分鐘過期，滑動過期
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        
        _memoryCache.Set(cacheKey, productDtos, cacheOptions);
        return productDtos;
    }
} 