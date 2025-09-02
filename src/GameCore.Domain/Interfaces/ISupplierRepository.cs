using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 供應商資料存取介面
/// 定義供應商相關的資料庫操作
/// </summary>
public interface ISupplierRepository
{
    /// <summary>
    /// 根據ID取得供應商
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>供應商資訊</returns>
    Task<Supplier?> GetByIdAsync(int supplierId);

    /// <summary>
    /// 新增供應商
    /// </summary>
    /// <param name="supplier">供應商資訊</param>
    /// <returns>供應商ID</returns>
    Task<int> AddAsync(Supplier supplier);

    /// <summary>
    /// 更新供應商
    /// </summary>
    /// <param name="supplier">供應商資訊</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(Supplier supplier);

    /// <summary>
    /// 刪除供應商
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int supplierId);

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    /// <returns>供應商列表</returns>
    Task<List<Supplier>> GetAllAsync();

    /// <summary>
    /// 根據名稱搜尋供應商
    /// </summary>
    /// <param name="name">供應商名稱</param>
    /// <returns>供應商列表</returns>
    Task<List<Supplier>> GetByNameAsync(string name);

    /// <summary>
    /// 根據狀態取得供應商列表
    /// </summary>
    /// <param name="isActive">是否啟用</param>
    /// <returns>供應商列表</returns>
    Task<List<Supplier>> GetByStatusAsync(bool isActive);

    /// <summary>
    /// 取得啟用的供應商列表
    /// </summary>
    /// <returns>啟用的供應商列表</returns>
    Task<List<Supplier>> GetActiveSuppliersAsync();
} 