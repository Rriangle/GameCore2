using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 商品資訊 Repository 實作
/// </summary>
public class ProductInfoRepository : IProductInfoRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<ProductInfoRepository> _logger;

    public ProductInfoRepository(GameCoreDbContext context, ILogger<ProductInfoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 根據ID取得商品
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品實體</returns>
    public async Task<ProductInfo?> GetByIdAsync(int productId)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能，避免 EF Core 變更追蹤開銷
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(p => p.ProductID == productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取商品時發生錯誤: {ProductId}", productId);
            throw;
        }
    }

    /// <summary>
    /// 新增商品
    /// </summary>
    /// <param name="product">商品實體</param>
    /// <returns>商品ID</returns>
    public async Task<int> AddAsync(ProductInfo product)
    {
        try
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.ProductCreatedAt = DateTime.UtcNow;

            _context.ProductInfos.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("新增商品成功: {ProductId}, {Name}", product.ProductID, product.ProductName);
            return product.ProductID;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增商品時發生錯誤: {Name}", product?.ProductName);
            throw;
        }
    }

    /// <summary>
    /// 更新商品
    /// </summary>
    /// <param name="product">商品實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(ProductInfo product)
    {
        try
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            product.ProductUpdatedAt = DateTime.UtcNow;

            _context.ProductInfos.Update(product);
            var result = await _context.SaveChangesAsync();

            _logger.LogInformation("更新商品成功: {ProductId}, {Name}", product.ProductID, product.ProductName);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新商品時發生錯誤: {ProductId}, {Name}", product?.ProductID, product?.ProductName);
            throw;
        }
    }

    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int productId)
    {
        try
        {
            var product = await _context.ProductInfos.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("要刪除的商品不存在: {ProductId}", productId);
                return false;
            }

            _context.ProductInfos.Remove(product);
            var result = await _context.SaveChangesAsync();

            _logger.LogInformation("刪除商品成功: {ProductId}, {Name}", productId, product.ProductName);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除商品時發生錯誤: {ProductId}", productId);
            throw;
        }
    }

    /// <summary>
    /// 根據名稱搜尋商品
    /// </summary>
    /// <param name="name">商品名稱</param>
    /// <returns>商品列表</returns>
    public async Task<List<ProductInfo>> SearchByNameAsync(string name)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：搜尋查詢通常不需要變更追蹤
                .Where(p => p.ProductName.Contains(name))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據名稱搜尋商品時發生錯誤: {Name}", name);
            throw;
        }
    }

    /// <summary>
    /// 根據類別取得商品
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <returns>商品列表</returns>
    public async Task<List<ProductInfo>> GetByCategoryAsync(string category)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：分類查詢通常不需要變更追蹤
                .Where(p => p.ProductType == category)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據類別取得商品時發生錯誤: {Category}", category);
            throw;
        }
    }

    /// <summary>
    /// 取得商品總數
    /// </summary>
    /// <returns>商品總數</returns>
    public async Task<int> GetCountAsync()
    {
        try
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            return await _context.ProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得商品總數時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 根據類別取得商品數量
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <returns>商品數量</returns>
    public async Task<int> GetCountByCategoryAsync(string category)
    {
        try
        {
            // 效能優化：計數查詢不需要 Include 和變更追蹤
            return await _context.ProductInfos
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .Where(p => p.ProductType == category)
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據類別取得商品數量時發生錯誤: {Category}", category);
            throw;
        }
    }

    /// <summary>
    /// 分頁取得商品
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>分頁商品列表</returns>
    public async Task<List<ProductInfo>> GetPagedAsync(int page, int pageSize)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：分頁查詢通常不需要變更追蹤
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分頁取得商品時發生錯誤: Page={Page}, PageSize={PageSize}", page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// 檢查商品是否存在
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>是否存在</returns>
    public async Task<bool> ExistsAsync(int productId)
    {
        try
        {
            // 效能優化：使用 AnyAsync 替代 CountAsync > 0，避免完整計數
            return await _context.ProductInfos
                .AsNoTracking() // 效能優化：存在性檢查不需要變更追蹤
                .AnyAsync(p => p.ProductID == productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查商品是否存在時發生錯誤: {ProductId}", productId);
            throw;
        }
    }

    /// <summary>
    /// 取得商品列表（支援分類、搜尋和分頁）
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>商品列表</returns>
    public async Task<List<ProductInfo>> GetProductsAsync(string? category, string? searchTerm, int page, int pageSize)
    {
        try
        {
            var query = _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking(); // 效能優化：列表查詢通常不需要變更追蹤

            // 套用分類篩選
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductType == category);
            }

            // 套用搜尋篩選
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }

            // 套用分頁
            return await query
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得商品列表時發生錯誤: Category={Category}, SearchTerm={SearchTerm}, Page={Page}, PageSize={PageSize}", 
                category, searchTerm, page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// 取得商品數量（支援分類和搜尋）
    /// </summary>
    /// <param name="category">商品類別</param>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <returns>商品數量</returns>
    public async Task<int> GetProductCountAsync(string? category, string? searchTerm)
    {
        try
        {
            var query = _context.ProductInfos.AsNoTracking(); // 效能優化：計數查詢不需要變更追蹤

            // 套用分類篩選
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductType == category);
            }

            // 套用搜尋篩選
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProductName.Contains(searchTerm));
            }

            return await query.CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得商品數量時發生錯誤: Category={Category}, SearchTerm={SearchTerm}", category, searchTerm);
            throw;
        }
    }

    /// <summary>
    /// 取得商品類別列表
    /// </summary>
    /// <returns>商品類別列表</returns>
    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 和 Distinct() 提升查詢效能
            return await _context.ProductInfos
                .AsNoTracking() // 效能優化：分類查詢不需要變更追蹤
                .Where(p => !string.IsNullOrEmpty(p.ProductType))
                .Select(p => p.ProductType)
                .Distinct()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得商品類別列表時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 取得熱門商品
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>熱門商品列表</returns>
    public async Task<List<ProductInfo>> GetPopularProductsAsync(int top)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：熱門商品查詢通常不需要變更追蹤
                .OrderByDescending(p => p.ProductCreatedAt) // 暫時使用建立時間排序，實際應根據銷量或評分
                .Take(top)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得熱門商品時發生錯誤: Top={Top}", top);
            throw;
        }
    }

    /// <summary>
    /// 根據供應商ID取得商品列表
    /// </summary>
    /// <param name="supplierId">供應商ID</param>
    /// <returns>商品列表</returns>
    public async Task<List<ProductInfo>> GetBySupplierIdAsync(int supplierId)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：供應商查詢通常不需要變更追蹤
                .Where(p => p.ProductCreatedBy == supplierId.ToString()) // 暫時使用 ProductCreatedBy 作為供應商ID
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據供應商ID取得商品時發生錯誤: SupplierId={SupplierId}", supplierId);
            throw;
        }
    }

    /// <summary>
    /// 根據價格範圍取得商品列表
    /// </summary>
    /// <param name="minPrice">最小價格</param>
    /// <param name="maxPrice">最大價格</param>
    /// <returns>商品列表</returns>
    public async Task<List<ProductInfo>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.ProductInfos
                .Include(p => p.GameProductDetails)
                .Include(p => p.OtherProductDetails)
                .AsNoTracking() // 效能優化：價格範圍查詢通常不需要變更追蹤
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據價格範圍取得商品時發生錯誤: MinPrice={MinPrice}, MaxPrice={MaxPrice}", minPrice, maxPrice);
            throw;
        }
    }

    /// <summary>
    /// 更新商品庫存
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="quantity">數量</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        try
        {
            var product = await _context.ProductInfos.FindAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("要更新庫存的商品不存在: {ProductId}", productId);
                return false;
            }

            // 更新庫存數量
            product.ShipmentQuantity = quantity;
            product.ProductUpdatedAt = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();

            _logger.LogInformation("更新商品庫存成功: {ProductId}, {Name}, 新庫存: {Quantity}", 
                productId, product.ProductName, quantity);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新商品庫存時發生錯誤: {ProductId}, {Quantity}", productId, quantity);
            throw;
        }
    }
} 