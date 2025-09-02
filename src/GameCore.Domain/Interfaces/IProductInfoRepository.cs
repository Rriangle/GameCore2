using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 商品資訊資料存取介面
/// 定義商品相關的資料庫操作
/// </summary>
public interface IProductInfoRepository
{
    /// <summary>
    /// 根據ID取得商品
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品資訊</returns>
    Task<ProductInfo?> GetByIdAsync(int productId);

    /// <summary>
    /// 新增商品
    /// </summary>
    /// <param name="product">商品資訊</param>
    /// <returns>商品ID</returns>
    Task<int> AddAsync(ProductInfo product);

    /// <summary>
    /// 更新商品
    /// </summary>
    /// <param name="product">商品資訊</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(ProductInfo product);

    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int productId);

    /// <summary>
    /// 取得商品列表
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetProductsAsync(string? category, string? searchTerm, int page, int pageSize);

    /// <summary>
    /// 取得商品數量
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <returns>商品數量</returns>
    Task<int> GetProductCountAsync(string? category, string? searchTerm);

    /// <summary>
    /// 取得商品類別列表
    /// </summary>
    /// <returns>商品類別列表</returns>
    Task<List<string>> GetCategoriesAsync();

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>熱門商品列表</returns>
    Task<List<ProductInfo>> GetPopularProductsAsync(int top);

    /// <summary>
    /// 根據供應商ID取得商品列表
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetBySupplierIdAsync(int supplierId);

    /// <summary>
    /// 根據價格範圍取得商品列表
    /// </summary>
    /// <param name="minPrice">最小價格</param>
    /// <param name="maxPrice">最大價格</param>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

    /// <summary>
    /// 更新商品庫存
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="quantity">數量</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStockAsync(int productId, int quantity);
} 