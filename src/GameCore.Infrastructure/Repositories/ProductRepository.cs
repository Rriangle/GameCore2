using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 商品資料存取實作，提供商品相關的資料庫操作
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly GameCoreDbContext _context;

    public ProductRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據編號取得商品 (包含詳細資訊和供應商)
    /// </summary>
    public async Task<ProductInfo?> GetByIdAsync(int productId)
    {
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    /// <summary>
    /// 搜尋商品 (分頁查詢)
    /// </summary>
    public async Task<PagedProductsDto> SearchProductsAsync(ProductSearchQueryDto query)
    {
        var queryable = _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .AsQueryable();

        // 關鍵字篩選 (商品名稱)
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(p => p.ProductName.Contains(query.Keyword));
        }

        // 商品類型篩選
        if (!string.IsNullOrWhiteSpace(query.ProductType))
        {
            queryable = queryable.Where(p => p.ProductType == query.ProductType);
        }

        // 供應商篩選
        if (query.SupplierId.HasValue)
        {
            queryable = queryable.Where(p => 
                (p.GameProductDetails != null && p.GameProductDetails.SupplierId == query.SupplierId) ||
                (p.OtherProductDetails != null && p.OtherProductDetails.SupplierId == query.SupplierId));
        }

        // 價格範圍篩選
        if (query.MinPrice.HasValue)
        {
            queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
        }
        if (query.MaxPrice.HasValue)
        {
            queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
        }

        // 排序
        queryable = query.SortBy?.ToLower() switch
        {
            "name_asc" => queryable.OrderBy(p => p.ProductName),
            "name_desc" => queryable.OrderByDescending(p => p.ProductName),
            "price_asc" => queryable.OrderBy(p => p.Price),
            "price_desc" => queryable.OrderByDescending(p => p.Price),
            "created_desc" => queryable.OrderByDescending(p => p.ProductCreatedAt),
            _ => queryable.OrderByDescending(p => p.ProductCreatedAt)
        };

        var totalCount = await queryable.CountAsync();
        var products = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductType = p.ProductType,
                Price = p.Price,
                CurrencyCode = p.CurrencyCode,
                ShipmentQuantity = p.ShipmentQuantity,
                ProductCreatedAt = p.ProductCreatedAt,
                ProductUpdatedAt = p.ProductUpdatedAt,
                ProductCreatedBy = p.Creator != null ? p.Creator.User_name : null,
                SupplierName = p.GameProductDetails != null ? p.GameProductDetails.Supplier.SupplierName :
                              p.OtherProductDetails != null ? p.OtherProductDetails.Supplier.SupplierName : null
            })
            .ToListAsync();

        return new PagedProductsDto
        {
            Products = products,
            TotalCount = totalCount,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 取得所有商品 (包含詳細資訊)
    /// </summary>
    public async Task<List<ProductInfo>> GetAllAsync()
    {
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .OrderBy(p => p.ProductId)
            .ToListAsync();
    }

    /// <summary>
    /// 根據商品類型取得商品列表
    /// </summary>
    public async Task<List<ProductInfo>> GetByProductTypeAsync(string productType)
    {
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .Where(p => p.ProductType == productType)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    /// <summary>
    /// 根據供應商取得商品列表
    /// </summary>
    public async Task<List<ProductInfo>> GetBySupplierIdAsync(int supplierId)
    {
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .Where(p => 
                (p.GameProductDetails != null && p.GameProductDetails.SupplierId == supplierId) ||
                (p.OtherProductDetails != null && p.OtherProductDetails.SupplierId == supplierId))
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    /// <summary>
    /// 建立新商品
    /// </summary>
    public async Task<ProductInfo> CreateAsync(ProductInfo product)
    {
        product.ProductCreatedAt = DateTime.UtcNow;
        product.ProductUpdatedAt = DateTime.UtcNow;

        _context.ProductInfos.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    /// <summary>
    /// 更新商品資料
    /// </summary>
    public async Task<ProductInfo> UpdateAsync(ProductInfo product)
    {
        product.ProductUpdatedAt = DateTime.UtcNow;

        _context.ProductInfos.Update(product);
        await _context.SaveChangesAsync();

        return product;
    }

    /// <summary>
    /// 刪除商品
    /// </summary>
    public async Task<bool> DeleteAsync(int productId)
    {
        var product = await _context.ProductInfos.FindAsync(productId);
        if (product == null)
            return false;

        _context.ProductInfos.Remove(product);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 檢查商品是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(int productId)
    {
        return await _context.ProductInfos.AnyAsync(p => p.ProductId == productId);
    }

    /// <summary>
    /// 檢查庫存是否足夠
    /// </summary>
    public async Task<bool> CheckStockAsync(int productId, int quantity)
    {
        var product = await _context.ProductInfos.FindAsync(productId);
        if (product == null)
            return false;

        // 檢查出貨量是否足夠
        return product.ShipmentQuantity >= quantity;
    }

    /// <summary>
    /// 更新庫存數量
    /// </summary>
    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await _context.ProductInfos.FindAsync(productId);
        if (product == null)
            return false;

        // 更新出貨量 (正數增加，負數減少)
        product.ShipmentQuantity += quantity;
        product.ProductUpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 取得熱門商品 (依銷售量或瀏覽量)
    /// </summary>
    public async Task<List<ProductInfo>> GetPopularProductsAsync(int limit = 10)
    {
        // 基於訂單明細數量來判斷熱門程度
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .Include(p => p.OrderItems)
            .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
            .Take(limit)
            .ToListAsync();
    }

    /// <summary>
    /// 取得低庫存商品
    /// </summary>
    public async Task<List<ProductInfo>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _context.ProductInfos
            .Include(p => p.Creator)
            .Include(p => p.GameProductDetails)
                .ThenInclude(g => g != null ? g.Supplier : null)
            .Include(p => p.OtherProductDetails)
                .ThenInclude(o => o != null ? o.Supplier : null)
            .Where(p => p.ShipmentQuantity <= threshold)
            .OrderBy(p => p.ShipmentQuantity)
            .ToListAsync();
    }
}