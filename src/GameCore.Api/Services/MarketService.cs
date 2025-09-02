using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

/// <summary>
/// 玩家市場服務
/// 處理玩家市場商品和訂單管理
/// </summary>
public class MarketService : IMarketService
{
    private readonly IPlayerMarketProductInfoRepository _productRepository;
    private readonly IPlayerMarketOrderInfoRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWalletService _walletService;

    public MarketService(
        IPlayerMarketProductInfoRepository productRepository,
        IPlayerMarketOrderInfoRepository orderRepository,
        IUserRepository userRepository,
        IWalletService walletService)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _walletService = walletService;
    }

    /// <summary>
    /// 搜尋市場商品
    /// </summary>
    /// <param name="request">搜尋請求</param>
    /// <returns>商品列表</returns>
    public async Task<PagedResult<MarketProductDto>> SearchProductsAsync(MarketSearchRequest request)
    {
        var query = _productRepository.GetQueryable()
            // 效能優化：唯讀查詢加入 AsNoTracking()，降低 EF 變更追蹤成本
            // 前後差異：不影響回傳資料，僅減少追蹤與分配
            // 風險與回滾：風險極低，若未來需追蹤請移除此設定
            .AsNoTracking()
            .Include(p => p.ProductImages)
            .Include(p => p.Seller)
            .Where(p => p.PStatus == "Active");

        // 套用搜尋條件
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(p => (p.PProductName != null && p.PProductName.Contains(request.Keyword)) || 
                                   (p.PProductTitle != null && p.PProductTitle.Contains(request.Keyword)) || 
                                   (p.PProductDescription != null && p.PProductDescription.Contains(request.Keyword)));
        }

        if (request.ProductId.HasValue)
        {
            query = query.Where(p => p.ProductID == request.ProductId.Value);
        }

        if (!string.IsNullOrEmpty(request.ProductType))
        {
            query = query.Where(p => p.PProductType == request.ProductType);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        if (request.SellerId.HasValue)
        {
            query = query.Where(p => p.SellerID == request.SellerId.Value);
        }

        // 套用排序
        query = query.OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var productDtos = new List<MarketProductDto>();
        foreach (var product in products)
        {
            var images = new List<string>();
            if (product.ProductImages != null)
            {
                foreach (var img in product.ProductImages)
                {
                    if (img.PProductImgURL != null)
                    {
                        images.Add(Convert.ToBase64String(img.PProductImgURL));
                    }
                }
            }

            productDtos.Add(new MarketProductDto
            {
                ProductId = product.PProductID,
                ProductType = product.PProductType,
                ProductTitle = product.PProductTitle,
                ProductName = product.PProductName,
                ProductDescription = product.PProductDescription ?? string.Empty,
                Price = product.Price,
                Status = product.PStatus,
                SellerId = product.SellerID,
                SellerName = product.Seller?.User_Name ?? string.Empty,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Images = images
            });
        }

        return new PagedResult<MarketProductDto>
        {
            Data = productDtos,
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳細資訊</returns>
    public async Task<MarketProductDto?> GetProductAsync(int productId)
    {
        var product = await _productRepository.GetQueryable()
            .Include(p => p.ProductImages)
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.PProductID == productId);

        if (product == null) return null;

        var images = new List<string>();
        if (product.ProductImages != null)
        {
            foreach (var img in product.ProductImages)
            {
                if (img.PProductImgURL != null)
                {
                    images.Add(Convert.ToBase64String(img.PProductImgURL));
                }
            }
        }

        return new MarketProductDto
        {
            ProductId = product.PProductID,
            ProductType = product.PProductType,
            ProductTitle = product.PProductTitle,
            ProductName = product.PProductName,
            ProductDescription = product.PProductDescription ?? string.Empty,
            Price = product.Price,
            Status = product.PStatus,
            SellerId = product.SellerID,
            SellerName = product.Seller?.User_Name ?? string.Empty,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Images = images
        };
    }

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="request">建立訂單請求</param>
    /// <param name="buyerId">買家ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<ServiceResult<MarketOrderDto>> CreateOrderAsync(CreateMarketOrderDto request, int buyerId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ProductID);
            if (product == null)
            {
                return ServiceResult<MarketOrderDto>.CreateFailure("商品不存在");
            }

            if (product.PStatus != "Active")
            {
                return ServiceResult<MarketOrderDto>.CreateFailure("商品已下架");
            }

            var totalAmount = product.Price * request.Quantity;
            var platformFee = totalAmount * 0.05m; // 5% 平台費用

            // 檢查買家餘額
            var wallet = await _walletService.GetWalletAsync(buyerId);
            if (wallet == null || wallet.Balance < totalAmount + platformFee)
            {
                return ServiceResult<MarketOrderDto>.CreateFailure("餘額不足");
            }

            // 扣除買家餘額
            var deductResult = await _walletService.DeductPointsAsync(buyerId, (int)(totalAmount + platformFee), "購買市場商品");
            if (!deductResult.Success)
            {
                return ServiceResult<MarketOrderDto>.CreateFailure("扣款失敗");
            }

            // 建立訂單
            var order = new PlayerMarketOrderInfo
            {
                PProductID = request.ProductID,
                SellerID = product.SellerID,
                BuyerID = buyerId,
                POrderDate = DateTime.UtcNow,
                POrderStatus = "Created",
                PPaymentStatus = "Paid",
                PUnitPrice = (int)product.Price,
                PQuantity = request.Quantity,
                POrderTotal = (int)(totalAmount + platformFee),
                POrderCreatedAt = DateTime.UtcNow,
                POrderUpdatedAt = DateTime.UtcNow,
                Platform_Fee = (int)platformFee
            };

            var createdOrder = await _orderRepository.AddAsync(order);
            if (createdOrder == null)
            {
                // 退款
                await _walletService.AddPointsAsync(buyerId, (int)(totalAmount + platformFee), "訂單建立失敗退款");
                return ServiceResult<MarketOrderDto>.CreateFailure("訂單建立失敗");
            }

            // 取得買賣家資訊
            var buyer = await _userRepository.GetByIdAsync(buyerId);
            var seller = await _userRepository.GetByIdAsync(product.SellerID);

            var orderDto = new MarketOrderDto
            {
                OrderID = createdOrder.POrderID,
                BuyerUserID = buyerId,
                SellerUserID = product.SellerID,
                ProductID = request.ProductID,
                ProductName = product.PProductName,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                TotalAmount = totalAmount + platformFee,
                PlatformFee = platformFee,
                Status = createdOrder.POrderStatus,
                CreatedAt = createdOrder.POrderCreatedAt,
                CompletedAt = null,
                BuyerName = buyer?.User_Name ?? string.Empty,
                SellerName = seller?.User_Name ?? string.Empty
            };

            return ServiceResult<MarketOrderDto>.CreateSuccess(orderDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<MarketOrderDto>.CreateFailure($"建立訂單失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得用戶訂單列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="role">角色 (buyer/seller)</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    public async Task<PagedResult<MarketOrderDto>> GetUserOrdersAsync(int userId, string role, int page = 1, int pageSize = 20)
    {
        IQueryable<PlayerMarketOrderInfo> query = _orderRepository.GetQueryable()
            // 效能優化：唯讀查詢加入 AsNoTracking()，避免不必要的追蹤
            // 前後差異：行為不變，降低記憶體使用量
            // 風險：極低
            .AsNoTracking();

        if (role.ToLower() == "buyer")
        {
            query = query.Where(o => o.BuyerID == userId);
        }
        else if (role.ToLower() == "seller")
        {
            query = query.Where(o => o.SellerID == userId);
        }

        var totalCount = await query.CountAsync();
        var orders = await query
            .Include(o => o.Product)
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .OrderByDescending(o => o.POrderCreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderDtos = orders.Select(o => new MarketOrderDto
        {
            OrderID = o.POrderID,
            BuyerUserID = o.BuyerID,
            SellerUserID = o.SellerID,
            ProductID = o.PProductID,
            ProductName = o.Product.PProductName,
            Quantity = o.PQuantity,
            UnitPrice = o.PUnitPrice,
            TotalAmount = o.POrderTotal,
            PlatformFee = o.Platform_Fee,
            Status = o.POrderStatus,
            CreatedAt = o.POrderCreatedAt,
            CompletedAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            BuyerName = o.Buyer?.User_Name ?? string.Empty,
            SellerName = o.Seller?.User_Name ?? string.Empty
        }).ToList();

        return new PagedResult<MarketOrderDto>
        {
            Data = orderDtos,
            Total = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 取得訂單詳細資訊
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>訂單詳細資訊</returns>
    public async Task<MarketOrderDto?> GetOrderAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetQueryable()
            .Include(o => o.Product)
            .Include(o => o.Buyer)
            .Include(o => o.Seller)
            .FirstOrDefaultAsync(o => o.POrderID == orderId && (o.BuyerID == userId || o.SellerID == userId));

        if (order == null) return null;

        return new MarketOrderDto
        {
            OrderID = order.POrderID,
            BuyerUserID = order.BuyerID,
            SellerUserID = order.SellerID,
            ProductID = order.PProductID,
            ProductName = order.Product.PProductName,
            Quantity = order.PQuantity,
            UnitPrice = order.PUnitPrice,
            TotalAmount = order.POrderTotal,
            PlatformFee = order.Platform_Fee,
            Status = order.POrderStatus,
            CreatedAt = order.POrderCreatedAt,
            CompletedAt = null, // 暫時設為 null
            BuyerName = order.Buyer?.User_Name ?? string.Empty,
            SellerName = order.Seller?.User_Name ?? string.Empty
        };
    }

    /// <summary>
    /// 取得市場排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>排行榜列表</returns>
    public async Task<List<MarketRankingDto>> GetMarketRankingAsync(int top = 10)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 簡化版本：返回空的排行榜
            // 實際實作中需要從資料庫查詢銷售統計
            return new List<MarketRankingDto>();
        }
        catch (Exception)
        {
            return new List<MarketRankingDto>();
        }
    }
}

 