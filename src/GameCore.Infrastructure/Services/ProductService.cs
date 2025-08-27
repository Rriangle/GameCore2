using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 產品服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class ProductService : IProductService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ProductService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 10;
    private const string ProductsCacheKey = "Products_All";
    private const string CategoriesCacheKey = "Categories_All";
    private const string ProductCacheKey = "Product_{0}";
    private const string CategoryProductsCacheKey = "CategoryProducts_{0}";
    private const string OfficialStoreCacheKey = "OfficialStore_Products";

    public ProductService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<ProductService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取所有產品 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        _logger.LogInformation("開始獲取所有產品");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(ProductsCacheKey, out IEnumerable<Product> cachedProducts))
            {
                _logger.LogDebug("從快取獲取所有產品，數量: {Count}", cachedProducts.Count());
                return cachedProducts;
            }

            // 從資料庫獲取
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category.SortOrder)
                .ThenBy(p => p.Name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(ProductsCacheKey, products, cacheOptions);

            _logger.LogInformation("成功獲取所有產品，數量: {Count}", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取所有產品時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取分類產品 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        _logger.LogInformation("開始獲取分類產品，分類ID: {CategoryId}", categoryId);

        try
        {
            // 輸入驗證
            if (categoryId <= 0)
            {
                _logger.LogWarning("無效的分類ID: {CategoryId}", categoryId);
                return Enumerable.Empty<Product>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(CategoryProductsCacheKey, categoryId);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product> cachedProducts))
            {
                _logger.LogDebug("從快取獲取分類產品，分類ID: {CategoryId}, 數量: {Count}", categoryId, cachedProducts.Count());
                return cachedProducts;
            }

            // 從資料庫獲取
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.Name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, products, cacheOptions);

            _logger.LogInformation("成功獲取分類產品，分類ID: {CategoryId}, 數量: {Count}", categoryId, products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取分類產品時發生錯誤，分類ID: {CategoryId}", categoryId);
            throw;
        }
    }

    /// <summary>
    /// 獲取官方商店產品 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Product>> GetOfficialStoreProductsAsync()
    {
        _logger.LogInformation("開始獲取官方商店產品");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(OfficialStoreCacheKey, out IEnumerable<Product> cachedProducts))
            {
                _logger.LogDebug("從快取獲取官方商店產品，數量: {Count}", cachedProducts.Count());
                return cachedProducts;
            }

            // 從資料庫獲取
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsOfficialStore && p.IsActive)
                .OrderBy(p => p.Category.SortOrder)
                .ThenBy(p => p.Name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(OfficialStoreCacheKey, products, cacheOptions);

            _logger.LogInformation("成功獲取官方商店產品，數量: {Count}", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取官方商店產品時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取產品分頁列表 - 新增方法，提高性能
    /// </summary>
    public async Task<PaginatedProductsDto> GetProductsPaginatedAsync(int page = 1, int pageSize = DefaultPageSize, int? categoryId = null, string? searchTerm = null, string sortBy = "Name", string sortDirection = "asc")
    {
        _logger.LogInformation("開始獲取產品分頁列表，頁碼: {Page}, 頁面大小: {PageSize}, 分類ID: {CategoryId}, 搜尋詞: {SearchTerm}, 排序: {SortBy} {SortDirection}", 
            page, pageSize, categoryId, searchTerm, sortBy, sortDirection);

        try
        {
            // 輸入驗證
            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {Error}", validationResult.ErrorMessage);
                return CreateEmptyPaginatedProducts();
            }

            // 建立查詢
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsNoTracking();

            // 套用分類篩選
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 套用搜尋篩選
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLowerInvariant();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) || 
                                       p.Description.ToLower().Contains(searchLower));
            }

            // 獲取總記錄數
            var totalCount = await query.CountAsync();

            // 套用動態排序
            query = ApplySorting(query, sortBy, sortDirection);

            // 套用分頁
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 轉換為 DTO
            var productDtos = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                IsOfficialStore = p.IsOfficialStore,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            var result = new PaginatedProductsDto
            {
                Products = productDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = page > 1,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            _logger.LogInformation("成功獲取產品分頁列表，頁碼: {Page}, 記錄數: {Count}", page, products.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取產品分頁列表時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 根據ID獲取產品 - 優化版本（含快取）
    /// </summary>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("開始根據ID獲取產品，產品ID: {ProductId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的產品ID: {ProductId}", id);
                return null;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ProductCacheKey, id);
            if (_memoryCache.TryGetValue(cacheKey, out Product cachedProduct))
            {
                _logger.LogDebug("從快取獲取產品，產品ID: {ProductId}", id);
                return cachedProduct;
            }

            // 從資料庫獲取
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product != null)
            {
                // 存入快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, product, cacheOptions);
            }

            _logger.LogInformation("成功獲取產品，產品ID: {ProductId}", id);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據ID獲取產品時發生錯誤，產品ID: {ProductId}", id);
            throw;
        }
    }

    /// <summary>
    /// 建立產品 - 優化版本
    /// </summary>
    public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("開始建立產品，產品名稱: {ProductName}", createProductDto.Name);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateProductRequest(createProductDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("建立產品請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var product = new Product
            {
                Name = createProductDto.Name.Trim(),
                Description = createProductDto.Description?.Trim(),
                Price = createProductDto.Price,
                StockQuantity = createProductDto.StockQuantity,
                CategoryId = createProductDto.CategoryId,
                ImageUrl = createProductDto.ImageUrl?.Trim(),
                IsOfficialStore = createProductDto.IsOfficialStore,
                CreatedBy = 1, // TODO: Get from current user context
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功建立產品，產品ID: {ProductId}, 名稱: {ProductName}", product.Id, product.Name);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立產品時發生錯誤，產品名稱: {ProductName}", createProductDto.Name);
            throw;
        }
    }

    /// <summary>
    /// 更新產品 - 優化版本
    /// </summary>
    public async Task<Product> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("開始更新產品，產品ID: {ProductId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的產品ID: {ProductId}", id);
                throw new ArgumentException("無效的產品ID");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("產品不存在，產品ID: {ProductId}", id);
                throw new ArgumentException("產品不存在");
            }

            // 更新產品欄位
            UpdateProductFields(product, updateProductDto);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功更新產品，產品ID: {ProductId}", id);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新產品時發生錯誤，產品ID: {ProductId}", id);
            throw;
        }
    }

    /// <summary>
    /// 刪除產品 - 優化版本
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("開始刪除產品，產品ID: {ProductId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的產品ID: {ProductId}", id);
                return false;
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("產品不存在，產品ID: {ProductId}", id);
                return false;
            }

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功刪除產品，產品ID: {ProductId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除產品時發生錯誤，產品ID: {ProductId}", id);
            return false;
        }
    }

    /// <summary>
    /// 更新庫存 - 優化版本
    /// </summary>
    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        _logger.LogInformation("開始更新產品庫存，產品ID: {ProductId}, 數量變化: {Quantity}", id, quantity);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的產品ID: {ProductId}", id);
                return false;
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("產品不存在，產品ID: {ProductId}", id);
                return false;
            }

            var newStock = Math.Max(0, product.StockQuantity + quantity);
            product.StockQuantity = newStock;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功更新產品庫存，產品ID: {ProductId}, 新庫存: {NewStock}", id, newStock);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新產品庫存時發生錯誤，產品ID: {ProductId}", id);
            return false;
        }
    }

    /// <summary>
    /// 獲取所有分類 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
    {
        _logger.LogInformation("開始獲取所有產品分類");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(CategoriesCacheKey, out IEnumerable<ProductCategory> cachedCategories))
            {
                _logger.LogDebug("從快取獲取所有產品分類，數量: {Count}", cachedCategories.Count());
                return cachedCategories;
            }

            // 從資料庫獲取
            var categories = await _context.ProductCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(CategoriesCacheKey, categories, cacheOptions);

            _logger.LogInformation("成功獲取所有產品分類，數量: {Count}", categories.Count);
            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取所有產品分類時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 建立分類 - 優化版本
    /// </summary>
    public async Task<ProductCategory> CreateCategoryAsync(CreateProductCategoryDto createCategoryDto)
    {
        _logger.LogInformation("開始建立產品分類，分類名稱: {CategoryName}", createCategoryDto.Name);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateCategoryRequest(createCategoryDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("建立分類請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var category = new ProductCategory
            {
                Name = createCategoryDto.Name.Trim(),
                Description = createCategoryDto.Description?.Trim(),
                IconUrl = createCategoryDto.IconUrl?.Trim(),
                SortOrder = createCategoryDto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功建立產品分類，分類ID: {CategoryId}, 名稱: {CategoryName}", category.Id, category.Name);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立產品分類時發生錯誤，分類名稱: {CategoryName}", createCategoryDto.Name);
            throw;
        }
    }

    /// <summary>
    /// 更新分類 - 優化版本
    /// </summary>
    public async Task<ProductCategory> UpdateCategoryAsync(int id, UpdateProductCategoryDto updateCategoryDto)
    {
        _logger.LogInformation("開始更新產品分類，分類ID: {CategoryId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的分類ID: {CategoryId}", id);
                throw new ArgumentException("無效的分類ID");
            }

            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("分類不存在，分類ID: {CategoryId}", id);
                throw new ArgumentException("分類不存在");
            }

            // 更新分類欄位
            UpdateCategoryFields(category, updateCategoryDto);
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功更新產品分類，分類ID: {CategoryId}", id);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新產品分類時發生錯誤，分類ID: {CategoryId}", id);
            throw;
        }
    }

    /// <summary>
    /// 刪除分類 - 優化版本
    /// </summary>
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        _logger.LogInformation("開始刪除產品分類，分類ID: {CategoryId}", id);

        try
        {
            // 輸入驗證
            if (id <= 0)
            {
                _logger.LogWarning("無效的分類ID: {CategoryId}", id);
                return false;
            }

            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("分類不存在，分類ID: {CategoryId}", id);
                return false;
            }

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearProductRelatedCache();

            _logger.LogInformation("成功刪除產品分類，分類ID: {CategoryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除產品分類時發生錯誤，分類ID: {CategoryId}", id);
            return false;
        }
    }

    /// <summary>
    /// 清除產品相關快取 - 新增方法
    /// </summary>
    public void ClearProductRelatedCache()
    {
        try
        {
            _memoryCache.Remove(ProductsCacheKey);
            _memoryCache.Remove(OfficialStoreCacheKey);
            _memoryCache.Remove(CategoriesCacheKey);

            // 清除特定產品的快取（需要遍歷所有快取鍵，這裡簡化處理）
            // 在實際應用中，可以使用更複雜的快取標籤系統

            _logger.LogDebug("已清除產品相關快取");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除產品相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證分頁參數 - 新增方法，提高可讀性
    /// </summary>
    private ValidationResult ValidatePaginationParameters(int page, int pageSize)
    {
        var result = new ValidationResult();

        if (page <= 0)
            result.AddError("頁碼必須大於0");

        if (pageSize <= 0)
            result.AddError("頁面大小必須大於0");

        if (pageSize > MaxPageSize)
            result.AddError($"頁面大小不能超過{MaxPageSize}");

        return result;
    }

    /// <summary>
    /// 驗證建立產品請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateProductRequest(CreateProductDto request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Name))
            result.AddError("產品名稱不能為空");

        if (request.Price <= 0)
            result.AddError("產品價格必須大於0");

        if (request.StockQuantity < 0)
            result.AddError("產品庫存不能為負數");

        if (request.CategoryId <= 0)
            result.AddError("產品分類ID必須大於0");

        return result;
    }

    /// <summary>
    /// 驗證建立分類請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateCategoryRequest(CreateProductCategoryDto request)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(request.Name))
            result.AddError("分類名稱不能為空");

        if (request.SortOrder < 0)
            result.AddError("分類排序不能為負數");

        return result;
    }

    /// <summary>
    /// 更新產品欄位 - 新增方法
    /// </summary>
    private void UpdateProductFields(Product product, UpdateProductDto updateDto)
    {
        if (updateDto.Name != null)
            product.Name = updateDto.Name.Trim();
        if (updateDto.Description != null)
            product.Description = updateDto.Description.Trim();
        if (updateDto.Price.HasValue)
            product.Price = updateDto.Price.Value;
        if (updateDto.StockQuantity.HasValue)
            product.StockQuantity = updateDto.StockQuantity.Value;
        if (updateDto.CategoryId.HasValue)
            product.CategoryId = updateDto.CategoryId.Value;
        if (updateDto.ImageUrl != null)
            product.ImageUrl = updateDto.ImageUrl.Trim();
        if (updateDto.IsActive.HasValue)
            product.IsActive = updateDto.IsActive.Value;
    }

    /// <summary>
    /// 更新分類欄位 - 新增方法
    /// </summary>
    private void UpdateCategoryFields(ProductCategory category, UpdateProductCategoryDto updateDto)
    {
        if (updateDto.Name != null)
            category.Name = updateDto.Name.Trim();
        if (updateDto.Description != null)
            category.Description = updateDto.Description.Trim();
        if (updateDto.IconUrl != null)
            category.IconUrl = updateDto.IconUrl.Trim();
        if (updateDto.SortOrder.HasValue)
            category.SortOrder = updateDto.SortOrder.Value;
        if (updateDto.IsActive.HasValue)
            category.IsActive = updateDto.IsActive.Value;
    }

    /// <summary>
    /// 建立空的分頁產品結果 - 新增方法
    /// </summary>
    private PaginatedProductsDto CreateEmptyPaginatedProducts()
    {
        return new PaginatedProductsDto
        {
            Products = new List<ProductResponseDto>(),
            Page = 1,
            PageSize = DefaultPageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false,
            SearchTerm = null,
            CategoryId = null,
            SortBy = "Name",
            SortDirection = "asc"
        };
    }

    /// <summary>
    /// 套用動態排序 - 新增方法
    /// </summary>
    private IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, string sortDirection)
    {
        var isAscending = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            "stockquantity" => isAscending ? query.OrderBy(p => p.StockQuantity) : query.OrderByDescending(p => p.StockQuantity),
            "createdat" => isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            "category" => isAscending ? query.OrderBy(p => p.Category.SortOrder).ThenBy(p => p.Name) : query.OrderByDescending(p => p.Category.SortOrder).ThenByDescending(p => p.Name),
            _ => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name) // 預設按名稱排序
        };
    }

    #endregion
}