using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 供應商資料存取介面，定義供應商相關的資料庫操作
/// </summary>
public interface ISupplierRepository
{
    /// <summary>
    /// 根據編號取得供應商
    /// </summary>
    /// <param name="supplierId">供應商編號</param>
    /// <returns>供應商資料，不存在則返回 null</returns>
    Task<Supplier?> GetByIdAsync(int supplierId);

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    /// <returns>供應商列表</returns>
    Task<List<Supplier>> GetAllAsync();

    /// <summary>
    /// 根據名稱搜尋供應商
    /// </summary>
    /// <param name="supplierName">供應商名稱 (支援部分比對)</param>
    /// <returns>符合的供應商列表</returns>
    Task<List<Supplier>> SearchByNameAsync(string supplierName);

    /// <summary>
    /// 建立新供應商
    /// </summary>
    /// <param name="supplier">供應商資料</param>
    /// <returns>建立的供應商資料</returns>
    Task<Supplier> CreateAsync(Supplier supplier);

    /// <summary>
    /// 更新供應商資料
    /// </summary>
    /// <param name="supplier">供應商資料</param>
    /// <returns>更新的供應商資料</returns>
    Task<Supplier> UpdateAsync(Supplier supplier);

    /// <summary>
    /// 刪除供應商
    /// </summary>
    /// <param name="supplierId">供應商編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int supplierId);

    /// <summary>
    /// 檢查供應商是否存在
    /// </summary>
    /// <param name="supplierId">供應商編號</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(int supplierId);

    /// <summary>
    /// 檢查供應商名稱是否已存在
    /// </summary>
    /// <param name="supplierName">供應商名稱</param>
    /// <param name="excludeSupplierId">排除的供應商編號 (用於更新時檢查)</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNameExistsAsync(string supplierName, int? excludeSupplierId = null);

    /// <summary>
    /// 取得供應商的商品數量統計
    /// </summary>
    /// <param name="supplierId">供應商編號</param>
    /// <returns>商品數量統計 (遊戲商品數量, 其他商品數量)</returns>
    Task<(int gameProductCount, int otherProductCount)> GetProductCountAsync(int supplierId);

    /// <summary>
    /// 取得有商品的供應商列表
    /// </summary>
    /// <returns>有商品的供應商列表</returns>
    Task<List<Supplier>> GetSuppliersWithProductsAsync();
}