using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 商品資料存取介面，定義商品相關的資料庫操作
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 根據編號取得商品 (包含詳細資訊和供應商)
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>商品資料，不存在則返回 null</returns>
    Task<ProductInfo?> GetByIdAsync(int productId);

    /// <summary>
    /// 搜尋商品 (分頁查詢)
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁商品列表</returns>
    Task<PagedProductsDto> SearchProductsAsync(ProductSearchQueryDto query);

    /// <summary>
    /// 取得所有商品 (包含詳細資訊)
    /// </summary>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetAllAsync();

    /// <summary>
    /// 根據商品類型取得商品列表
    /// </summary>
    /// <param name="productType">商品類型</param>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetByProductTypeAsync(string productType);

    /// <summary>
    /// 根據供應商取得商品列表
    /// </summary>
    /// <param name="supplierId">供應商編號</param>
    /// <returns>商品列表</returns>
    Task<List<ProductInfo>> GetBySupplierIdAsync(int supplierId);

    /// <summary>
    /// 建立新商品
    /// </summary>
    /// <param name="product">商品資料</param>
    /// <returns>建立的商品資料</returns>
    Task<ProductInfo> CreateAsync(ProductInfo product);

    /// <summary>
    /// 更新商品資料
    /// </summary>
    /// <param name="product">商品資料</param>
    /// <returns>更新的商品資料</returns>
    Task<ProductInfo> UpdateAsync(ProductInfo product);

    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int productId);

    /// <summary>
    /// 檢查商品是否存在
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(int productId);

    /// <summary>
    /// 檢查庫存是否足夠
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">需要數量</param>
    /// <returns>是否庫存足夠</returns>
    Task<bool> CheckStockAsync(int productId, int quantity);

    /// <summary>
    /// 更新庫存數量
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">異動數量 (正數增加，負數減少)</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateStockAsync(int productId, int quantity);

    /// <summary>
    /// 取得熱門商品 (依銷售量或瀏覽量)
    /// </summary>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門商品列表</returns>
    Task<List<ProductInfo>> GetPopularProductsAsync(int limit = 10);

    /// <summary>
    /// 取得低庫存商品
    /// </summary>
    /// <param name="threshold">庫存門檻</param>
    /// <returns>低庫存商品列表</returns>
    Task<List<ProductInfo>> GetLowStockProductsAsync(int threshold = 10);
}