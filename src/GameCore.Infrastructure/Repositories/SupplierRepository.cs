using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 供應商資料存取層
/// 實作 ISupplierRepository 介面
/// </summary>
public class SupplierRepository : ISupplierRepository
{
    private readonly GameCoreDbContext _context;

    public SupplierRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得供應商
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>供應商資訊</returns>
    public async Task<Supplier?> GetByIdAsync(int supplierId)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.SupplierID == supplierId);
    }

    /// <summary>
    /// 新增供應商
    /// </summary>
    /// <param name="supplier">供應商資訊</param>
    /// <returns>供應商ID</returns>
    public async Task<int> AddAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier.SupplierID;
    }

    /// <summary>
    /// 更新供應商
    /// </summary>
    /// <param name="supplier">供應商資訊</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除供應商
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(int supplierId)
    {
        var supplier = await GetByIdAsync(supplierId);
        if (supplier == null) return false;

        _context.Suppliers.Remove(supplier);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    /// <returns>供應商列表</returns>
    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    /// <summary>
    /// 根據名稱搜尋供應商
    /// </summary>
    /// <param name="name">供應商名稱</param>
    /// <returns>供應商列表</returns>
    public async Task<List<Supplier>> GetByNameAsync(string name)
    {
        return await _context.Suppliers
            .Where(s => s.SupplierName.Contains(name))
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    /// <summary>
    /// 根據狀態取得供應商列表
    /// </summary>
    /// <param name="isActive">是否啟用</param>
    /// <returns>供應商列表</returns>
    public async Task<List<Supplier>> GetByStatusAsync(bool isActive)
    {
        // 由於 Supplier 實體沒有 IsActive 屬性，暫時返回所有供應商
        // 可以根據實際需求調整邏輯
        return await _context.Suppliers
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    /// <summary>
    /// 取得啟用的供應商列表
    /// </summary>
    /// <returns>啟用的供應商列表</returns>
    public async Task<List<Supplier>> GetActiveSuppliersAsync()
    {
        // 由於 Supplier 實體沒有 IsActive 屬性，返回所有供應商
        return await _context.Suppliers
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }
} 