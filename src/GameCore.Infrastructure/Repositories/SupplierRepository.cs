using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 供應商資料存取實作，提供供應商相關的資料庫操作
/// </summary>
public class SupplierRepository : ISupplierRepository
{
    private readonly GameCoreDbContext _context;

    public SupplierRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據編號取得供應商
    /// </summary>
    public async Task<Supplier?> GetByIdAsync(int supplierId)
    {
        return await _context.Suppliers
            .Include(s => s.GameProducts)
            .Include(s => s.OtherProducts)
            .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
    }

    /// <summary>
    /// 取得所有供應商
    /// </summary>
    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .Include(s => s.GameProducts)
            .Include(s => s.OtherProducts)
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    /// <summary>
    /// 根據名稱搜尋供應商
    /// </summary>
    public async Task<List<Supplier>> SearchByNameAsync(string supplierName)
    {
        return await _context.Suppliers
            .Include(s => s.GameProducts)
            .Include(s => s.OtherProducts)
            .Where(s => s.SupplierName.Contains(supplierName))
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }

    /// <summary>
    /// 建立新供應商
    /// </summary>
    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    /// <summary>
    /// 更新供應商資料
    /// </summary>
    public async Task<Supplier> UpdateAsync(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    /// <summary>
    /// 刪除供應商
    /// </summary>
    public async Task<bool> DeleteAsync(int supplierId)
    {
        var supplier = await _context.Suppliers.FindAsync(supplierId);
        if (supplier == null)
            return false;

        _context.Suppliers.Remove(supplier);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 檢查供應商是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(int supplierId)
    {
        return await _context.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
    }

    /// <summary>
    /// 檢查供應商名稱是否已存在
    /// </summary>
    public async Task<bool> IsNameExistsAsync(string supplierName, int? excludeSupplierId = null)
    {
        var query = _context.Suppliers.Where(s => s.SupplierName == supplierName);
        
        if (excludeSupplierId.HasValue)
        {
            query = query.Where(s => s.SupplierId != excludeSupplierId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// 取得供應商的商品數量統計
    /// </summary>
    public async Task<(int gameProductCount, int otherProductCount)> GetProductCountAsync(int supplierId)
    {
        var gameProductCount = await _context.GameProductDetails
            .CountAsync(g => g.SupplierId == supplierId);

        var otherProductCount = await _context.OtherProductDetails
            .CountAsync(o => o.SupplierId == supplierId);

        return (gameProductCount, otherProductCount);
    }

    /// <summary>
    /// 取得有商品的供應商列表
    /// </summary>
    public async Task<List<Supplier>> GetSuppliersWithProductsAsync()
    {
        return await _context.Suppliers
            .Include(s => s.GameProducts)
            .Include(s => s.OtherProducts)
            .Where(s => s.GameProducts.Any() || s.OtherProducts.Any())
            .OrderBy(s => s.SupplierName)
            .ToListAsync();
    }
}