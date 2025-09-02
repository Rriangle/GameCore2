using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 玩家市場服務介面
/// </summary>
public interface IMarketService
{
    /// <summary>
    /// 搜尋市場商品
    /// </summary>
    /// <param name="request">搜尋請求</param>
    /// <returns>商品列表</returns>
    Task<PagedResult<MarketProductDto>> SearchProductsAsync(MarketSearchRequest request);

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳細資訊</returns>
    Task<MarketProductDto?> GetProductAsync(int productId);

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="request">建立訂單請求</param>
    /// <param name="buyerId">買家ID</param>
    /// <returns>訂單資訊</returns>
    Task<ServiceResult<MarketOrderDto>> CreateOrderAsync(CreateMarketOrderDto request, int buyerId);

    /// <summary>
    /// 取得用戶訂單列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="role">角色 (buyer/seller)</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    Task<PagedResult<MarketOrderDto>> GetUserOrdersAsync(int userId, string role, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得訂單詳細資訊
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>訂單詳細資訊</returns>
    Task<MarketOrderDto?> GetOrderAsync(int orderId, int userId);

    /// <summary>
    /// 取得市場排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>排行榜列表</returns>
    Task<List<MarketRankingDto>> GetMarketRankingAsync(int top = 10);
} 