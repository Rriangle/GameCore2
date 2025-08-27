using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 商城服務介面，定義商城相關的業務邏輯
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>商品詳細資訊，不存在則返回 null</returns>
    Task<ProductDto?> GetProductAsync(int productId);

    /// <summary>
    /// 搜尋商品 (分頁查詢)
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁商品列表</returns>
    Task<PagedProductsDto> SearchProductsAsync(ProductSearchQueryDto query);

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門商品列表</returns>
    Task<List<ProductDto>> GetPopularProductsAsync(int limit = 10);

    /// <summary>
    /// 取得商品推薦 (依用戶購買歷史)
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="limit">取得數量</param>
    /// <returns>推薦商品列表</returns>
    Task<List<ProductDto>> GetRecommendedProductsAsync(int userId, int limit = 10);

    /// <summary>
    /// 加入購物車
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">數量</param>
    /// <returns>是否加入成功</returns>
    Task<bool> AddToCartAsync(int userId, int productId, int quantity);

    /// <summary>
    /// 移除購物車商品
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="productId">商品編號</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveFromCartAsync(int userId, int productId);

    /// <summary>
    /// 更新購物車商品數量
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">新數量</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateCartQuantityAsync(int userId, int productId, int quantity);

    /// <summary>
    /// 取得購物車
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <returns>購物車資料</returns>
    Task<ShoppingCartDto> GetShoppingCartAsync(int userId);

    /// <summary>
    /// 清空購物車
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <returns>是否清空成功</returns>
    Task<bool> ClearCartAsync(int userId);

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="request">建立訂單請求</param>
    /// <returns>建立的訂單資料</returns>
    Task<OrderDto> CreateOrderAsync(CreateOrderRequestDto request);

    /// <summary>
    /// 取得訂單詳細資訊
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="userId">會員編號 (用於驗證權限)</param>
    /// <returns>訂單詳細資訊，不存在或無權限則返回 null</returns>
    Task<OrderDto?> GetOrderAsync(int orderId, int userId);

    /// <summary>
    /// 取得會員訂單列表
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁訂單列表</returns>
    Task<PagedOrdersDto> GetUserOrdersAsync(int userId, OrderSearchQueryDto query);

    /// <summary>
    /// 處理付款回呼
    /// </summary>
    /// <param name="callback">付款回呼資料</param>
    /// <returns>處理結果</returns>
    Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDto callback);

    /// <summary>
    /// 取得商城排行榜
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>排行榜資料</returns>
    Task<List<StoreRankingDto>> GetStoreRankingsAsync(RankingQueryDto query);

    /// <summary>
    /// 更新商城排行榜 (後台定時任務使用)
    /// </summary>
    /// <param name="periodType">期間類型</param>
    /// <param name="targetDate">目標日期</param>
    /// <returns>更新的記錄數量</returns>
    Task<int> UpdateStoreRankingsAsync(string periodType, DateTime targetDate);

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    /// <returns>供應商列表</returns>
    Task<List<SupplierDto>> GetSuppliersAsync();

    /// <summary>
    /// 檢查庫存是否足夠
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">需要數量</param>
    /// <returns>是否庫存足夠</returns>
    Task<bool> CheckStockAsync(int productId, int quantity);

    /// <summary>
    /// 取得商品詳細類型資訊 (遊戲商品或其他商品)
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>商品詳細類型資訊</returns>
    Task<ProductDetailsDto?> GetProductDetailsAsync(int productId);
}