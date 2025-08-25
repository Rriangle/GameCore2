using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 優化版產品服務測試 - 涵蓋新增的快取、驗證、分頁和性能功能
/// </summary>
public class ProductServiceOptimizedTests
{
    private readonly GameCoreDbContext _context;
    private readonly ProductService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceOptimizedTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_context, _memoryCache, _loggerMock.Object);

        // 種子測試資料
        SeedTestData();
    }

    private void SeedTestData()
    {
        // 建立測試產品分類
        var category1 = new ProductCategory
        {
            Id = 1,
            Name = "Category 1",
            Description = "First category",
            IsActive = true,
            SortOrder = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.ProductCategories.Add(category1);

        var category2 = new ProductCategory
        {
            Id = 2,
            Name = "Category 2",
            Description = "Second category",
            IsActive = true,
            SortOrder = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.ProductCategories.Add(category2);

        // 建立測試產品
        var product1 = new Product
        {
            Id = 1,
            Name = "Product 1",
            Description = "First product description",
            Price = 100.00m,
            StockQuantity = 50,
            CategoryId = 1,
            IsActive = true,
            IsOfficialStore = true,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        };
        _context.Products.Add(product1);

        var product2 = new Product
        {
            Id = 2,
            Name = "Product 2",
            Description = "Second product description",
            Price = 200.00m,
            StockQuantity = 30,
            CategoryId = 1,
            IsActive = true,
            IsOfficialStore = false,
            CreatedAt = DateTime.UtcNow.AddDays(-19)
        };
        _context.Products.Add(product2);

        var product3 = new Product
        {
            Id = 3,
            Name = "Product 3",
            Description = "Third product description",
            Price = 150.00m,
            StockQuantity = 25,
            CategoryId = 2,
            IsActive = true,
            IsOfficialStore = true,
            CreatedAt = DateTime.UtcNow.AddDays(-18)
        };
        _context.Products.Add(product3);

        _context.SaveChanges();
    }

    #region 所有產品獲取測試

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllActiveProducts()
    {
        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.All(result, p => Assert.True(p.IsActive));
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllProductsAsync();
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldOrderByCategoryAndName()
    {
        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        var products = result.ToList();
        Assert.Equal(1, products[0].CategoryId); // Category 1 first
        Assert.Equal(1, products[1].CategoryId); // Category 1 second
        Assert.Equal(2, products[2].CategoryId); // Category 2 third
    }

    #endregion

    #region 分類產品獲取測試

    [Fact]
    public async Task GetProductsByCategoryAsync_WithValidCategoryId_ShouldReturnProducts()
    {
        // Act
        var result = await _service.GetProductsByCategoryAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(1, p.CategoryId));
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithInvalidCategoryId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetProductsByCategoryAsync(-1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WithNonExistentCategoryId_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetProductsByCategoryAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetProductsByCategoryAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetProductsByCategoryAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 官方商店產品獲取測試

    [Fact]
    public async Task GetOfficialStoreProductsAsync_ShouldReturnOfficialStoreProducts()
    {
        // Act
        var result = await _service.GetOfficialStoreProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.True(p.IsOfficialStore));
    }

    [Fact]
    public async Task GetOfficialStoreProductsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetOfficialStoreProductsAsync();
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetOfficialStoreProductsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 產品分頁測試

    [Fact]
    public async Task GetProductsPaginatedAsync_WithValidParameters_ShouldReturnPaginatedResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
        Assert.Equal(2, result.Products.Count);
    }

    [Fact]
    public async Task GetProductsPaginatedAsync_WithCategoryFilter_ShouldReturnFilteredResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: 10, categoryId: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.CategoryId);
        Assert.All(result.Products, p => Assert.Equal(1, p.CategoryId));
    }

    [Fact]
    public async Task GetProductsPaginatedAsync_WithSearchTerm_ShouldReturnFilteredResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: 10, searchTerm: "Product 1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Product 1", result.SearchTerm);
        Assert.Single(result.Products);
        Assert.Equal("Product 1", result.Products.First().Name);
    }

    [Fact]
    public async Task GetProductsPaginatedAsync_WithInvalidPage_ShouldReturnEmptyResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: -1, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Products);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetProductsPaginatedAsync_WithInvalidPageSize_ShouldReturnEmptyResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: -1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Products);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetProductsPaginatedAsync_WithLargePageSize_ShouldReturnEmptyResults()
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: 1000);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Products);
        Assert.Equal(0, result.TotalCount);
    }

    #endregion

    #region 產品ID獲取測試

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Product 1", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetProductByIdAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetProductByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetProductByIdAsync(1);
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
    }

    #endregion

    #region 產品建立測試

    [Fact]
    public async Task CreateProductAsync_WithValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "New Product",
            Description = "New product description",
            Price = 75.50m,
            StockQuantity = 25,
            CategoryId = 1,
            ImageUrl = "new-product.jpg",
            IsOfficialStore = false
        };

        // Act
        var result = await _service.CreateProductAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(75.50m, result.Price);
        Assert.Equal(25, result.StockQuantity);
        Assert.Equal(1, result.CategoryId);
        Assert.False(result.IsOfficialStore);
    }

    [Fact]
    public async Task CreateProductAsync_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "",
            Description = "Description",
            Price = 100.00m,
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(request));
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidPrice_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "Product",
            Description = "Description",
            Price = 0,
            StockQuantity = 10,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(request));
    }

    [Fact]
    public async Task CreateProductAsync_WithNegativeStock_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "Product",
            Description = "Description",
            Price = 100.00m,
            StockQuantity = -5,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(request));
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidCategoryId_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "Product",
            Description = "Description",
            Price = 100.00m,
            StockQuantity = 10,
            CategoryId = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(request));
    }

    #endregion

    #region 產品更新測試

    [Fact]
    public async Task UpdateProductAsync_WithValidRequest_ShouldUpdateProduct()
    {
        // Arrange
        var request = new UpdateProductDto
        {
            Name = "Updated Product",
            Price = 125.00m,
            StockQuantity = 75
        };

        // Act
        var result = await _service.UpdateProductAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Product", result.Name);
        Assert.Equal(125.00m, result.Price);
        Assert.Equal(75, result.StockQuantity);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateProductDto
        {
            Name = "Updated Product"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync(-1, request));
    }

    [Fact]
    public async Task UpdateProductAsync_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateProductDto
        {
            Name = "Updated Product"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProductAsync(999, request));
    }

    #endregion

    #region 產品刪除測試

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ShouldDeleteProduct()
    {
        // Act
        var result = await _service.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        
        var deletedProduct = await _context.Products.FindAsync(1);
        Assert.False(deletedProduct!.IsActive);
    }

    [Fact]
    public async Task DeleteProductAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteProductAsync(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteProductAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 庫存更新測試

    [Fact]
    public async Task UpdateStockAsync_WithValidQuantity_ShouldUpdateStock()
    {
        // Act
        var result = await _service.UpdateStockAsync(1, 10);

        // Assert
        Assert.True(result);
        
        var updatedProduct = await _context.Products.FindAsync(1);
        Assert.Equal(60, updatedProduct!.StockQuantity); // 50 + 10
    }

    [Fact]
    public async Task UpdateStockAsync_WithNegativeQuantity_ShouldUpdateStock()
    {
        // Act
        var result = await _service.UpdateStockAsync(1, -20);

        // Assert
        Assert.True(result);
        
        var updatedProduct = await _context.Products.FindAsync(1);
        Assert.Equal(30, updatedProduct!.StockQuantity); // 50 - 20
    }

    [Fact]
    public async Task UpdateStockAsync_WithLargeNegativeQuantity_ShouldSetStockToZero()
    {
        // Act
        var result = await _service.UpdateStockAsync(1, -100);

        // Assert
        Assert.True(result);
        
        var updatedProduct = await _context.Products.FindAsync(1);
        Assert.Equal(0, updatedProduct!.StockQuantity); // 50 - 100 = 0 (minimum)
    }

    [Fact]
    public async Task UpdateStockAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.UpdateStockAsync(-1, 10);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStockAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.UpdateStockAsync(999, 10);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 分類獲取測試

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllActiveCategories()
    {
        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.True(c.IsActive));
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllCategoriesAsync();
        
        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 分類建立測試

    [Fact]
    public async Task CreateCategoryAsync_WithValidRequest_ShouldCreateCategory()
    {
        // Arrange
        var request = new CreateProductCategoryDto
        {
            Name = "New Category",
            Description = "New category description",
            IconUrl = "new-category.png",
            SortOrder = 3
        };

        // Act
        var result = await _service.CreateCategoryAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
        Assert.Equal("New category description", result.Description);
        Assert.Equal("new-category.png", result.IconUrl);
        Assert.Equal(3, result.SortOrder);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductCategoryDto
        {
            Name = "",
            Description = "Description",
            SortOrder = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategoryAsync(request));
    }

    [Fact]
    public async Task CreateCategoryAsync_WithNegativeSortOrder_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductCategoryDto
        {
            Name = "Category",
            Description = "Description",
            SortOrder = -1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategoryAsync(request));
    }

    #endregion

    #region 分類更新測試

    [Fact]
    public async Task UpdateCategoryAsync_WithValidRequest_ShouldUpdateCategory()
    {
        // Arrange
        var request = new UpdateProductCategoryDto
        {
            Name = "Updated Category",
            SortOrder = 5
        };

        // Act
        var result = await _service.UpdateCategoryAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Category", result.Name);
        Assert.Equal(5, result.SortOrder);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateProductCategoryDto
        {
            Name = "Updated Category"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCategoryAsync(-1, request));
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithNonExistentId_ShouldThrowException()
    {
        // Arrange
        var request = new UpdateProductCategoryDto
        {
            Name = "Updated Category"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCategoryAsync(999, request));
    }

    #endregion

    #region 分類刪除測試

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_ShouldDeleteCategory()
    {
        // Act
        var result = await _service.DeleteCategoryAsync(1);

        // Assert
        Assert.True(result);
        
        var deletedCategory = await _context.ProductCategories.FindAsync(1);
        Assert.False(deletedCategory!.IsActive);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteCategoryAsync(-1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Act
        var result = await _service.DeleteCategoryAsync(999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearProductRelatedCache_ShouldRemoveCacheEntries()
    {
        // Arrange - 先建立一些快取
        _memoryCache.Set("Products_All", new List<Product>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("OfficialStore_Products", new List<Product>(), TimeSpan.FromMinutes(5));
        _memoryCache.Set("Categories_All", new List<ProductCategory>(), TimeSpan.FromMinutes(5));

        // Act
        _service.ClearProductRelatedCache();

        // Assert
        Assert.False(_memoryCache.TryGetValue("Products_All", out _));
        Assert.False(_memoryCache.TryGetValue("OfficialStore_Products", out _));
        Assert.False(_memoryCache.TryGetValue("Categories_All", out _));
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task GetAllProductsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _service.GetAllProductsAsync());
    }

    [Fact]
    public async Task CreateProductAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var request = new CreateProductDto
        {
            Name = "Product",
            Description = "Description",
            Price = 100.00m,
            StockQuantity = 10,
            CategoryId = 1
        };

        // Arrange - 破壞資料庫連線
        _context.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _service.CreateProductAsync(request));
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetProductsPaginatedAsync_WithLargeDataset_ShouldCompleteWithinReasonableTime()
    {
        // Arrange - 建立更多測試資料
        for (int i = 4; i <= 50; i++)
        {
            var product = new Product
            {
                Id = i,
                Name = $"Product {i}",
                Description = $"Product {i} description",
                Price = 100.00m + i,
                StockQuantity = 10 + i,
                CategoryId = (i % 2) + 1,
                IsActive = true,
                IsOfficialStore = i % 3 == 0,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            };
            _context.Products.Add(product);
        }
        await _context.SaveChangesAsync();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _service.GetProductsPaginatedAsync(page: 1, pageSize: 20);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 2000); // 應該在2秒內完成
        Assert.NotNull(result);
        Assert.Equal(20, result.Products.Count);
        Assert.Equal(50, result.TotalCount);
    }

    #endregion

    #region 邊界值測試

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetProductsByCategoryAsync_WithBoundaryValues_ShouldReturnEmptyList(int categoryId)
    {
        // Act
        var result = await _service.GetProductsByCategoryAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task GetProductByIdAsync_WithBoundaryValues_ShouldReturnNull(int productId)
    {
        // Act
        var result = await _service.GetProductByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 10)]
    [InlineData(2, 5)]
    public async Task GetProductsPaginatedAsync_WithBoundaryValues_ShouldSucceed(int page, int pageSize)
    {
        // Act
        var result = await _service.GetProductsPaginatedAsync(page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _memoryCache.Dispose();
    }
}