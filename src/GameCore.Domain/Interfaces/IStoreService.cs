using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 官方商店服務介面
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// 取得商品列表
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>商品列表</returns>
    Task<GameCore.Shared.DTOs.PagedResult<ProductDto>> GetProductsAsync(
        string? category = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// 取得商品詳細資訊
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品詳細資訊</returns>
    Task<ProductDetailDto?> GetProductDetailAsync(int productId);

    /// <summary>
    /// 建立訂單
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="request">訂單請求</param>
    /// <returns>訂單結果</returns>
    Task<ServiceResult<OrderDto>> CreateOrderAsync(int userId, CreateOrderRequest request);

    /// <summary>
    /// 取得使用者訂單列表
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    Task<GameCore.Shared.DTOs.PagedResult<OrderDto>> GetUserOrdersAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得商品類別列表
    /// </summary>
    /// <returns>商品類別列表</returns>
    Task<List<string>> GetProductCategoriesAsync();

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>熱門商品列表</returns>
    Task<List<ProductDto>> GetPopularProductsAsync(int top = 10);
} 