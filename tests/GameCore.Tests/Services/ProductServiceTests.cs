using Microsoft.EntityFrameworkCore;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Services;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using Xunit;

namespace GameCore.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly GameCoreDbContext _context;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<GameCoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameCoreDbContext(options);
            _productService = new ProductService(_context);
            
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Create test categories
            var category1 = new ProductCategory
            {
                Id = 1,
                Name = "Electronics",
                Description = "Electronic devices and accessories",
                IconUrl = "/icons/electronics.png",
                SortOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var category2 = new ProductCategory
            {
                Id = 2,
                Name = "Clothing",
                Description = "Apparel and fashion items",
                IconUrl = "/icons/clothing.png",
                SortOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductCategories.AddRange(category1, category2);

            // Create test products
            var product1 = new Product
            {
                Id = 1,
                Name = "Gaming Mouse",
                Description = "High-performance gaming mouse with RGB lighting",
                Price = 59.99m,
                StockQuantity = 50,
                CategoryId = 1,
                ImageUrl = "/images/gaming-mouse.jpg",
                IsActive = true,
                IsOfficialStore = true,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            var product2 = new Product
            {
                Id = 2,
                Name = "Gaming Keyboard",
                Description = "Mechanical gaming keyboard with customizable keys",
                Price = 129.99m,
                StockQuantity = 30,
                CategoryId = 1,
                ImageUrl = "/images/gaming-keyboard.jpg",
                IsActive = true,
                IsOfficialStore = true,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            var product3 = new Product
            {
                Id = 3,
                Name = "Gaming T-Shirt",
                Description = "Comfortable gaming-themed t-shirt",
                Price = 24.99m,
                StockQuantity = 100,
                CategoryId = 2,
                ImageUrl = "/images/gaming-tshirt.jpg",
                IsActive = true,
                IsOfficialStore = false,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.AddRange(product1, product2, product3);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnAllActiveProducts()
        {
            // Act
            var products = await _productService.GetAllProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(3, products.Count());
            Assert.All(products, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetOfficialStoreProducts_ShouldReturnOnlyOfficialStoreProducts()
        {
            // Act
            var products = await _productService.GetOfficialStoreProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.All(products, p => Assert.True(p.IsOfficialStore));
        }

        [Fact]
        public async Task GetProductsByCategory_ShouldReturnProductsInSpecificCategory()
        {
            // Act
            var products = await _productService.GetProductsByCategoryAsync(1);

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.All(products, p => Assert.Equal(1, p.CategoryId));
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Act
            var product = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(product);
            Assert.Equal("Gaming Mouse", product.Name);
            Assert.Equal(59.99m, product.Price);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var product = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(product);
        }

        [Fact]
        public async Task CreateProduct_ShouldCreateNewProduct()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Name = "New Gaming Headset",
                Description = "High-quality gaming headset with noise cancellation",
                Price = 89.99m,
                StockQuantity = 25,
                CategoryId = 1,
                ImageUrl = "/images/gaming-headset.jpg",
                IsOfficialStore = true
            };

            // Act
            var product = await _productService.CreateProductAsync(createProductDto);

            // Assert
            Assert.NotNull(product);
            Assert.Equal("New Gaming Headset", product.Name);
            Assert.Equal(89.99m, product.Price);
            Assert.Equal(25, product.StockQuantity);
            Assert.Equal(1, product.CategoryId);
            Assert.True(product.IsOfficialStore);
            Assert.True(product.IsActive);
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateExistingProduct()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Name = "Updated Gaming Mouse",
                Price = 69.99m,
                StockQuantity = 75
            };

            // Act
            var product = await _productService.UpdateProductAsync(1, updateProductDto);

            // Assert
            Assert.NotNull(product);
            Assert.Equal("Updated Gaming Mouse", product.Name);
            Assert.Equal(69.99m, product.Price);
            Assert.Equal(75, product.StockQuantity);
            Assert.NotNull(product.UpdatedAt);
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Name = "Non-existent Product"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _productService.UpdateProductAsync(999, updateProductDto));
        }

        [Fact]
        public async Task DeleteProduct_ShouldDeactivateProduct()
        {
            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            
            var deletedProduct = await _context.Products.FindAsync(1);
            Assert.NotNull(deletedProduct);
            Assert.False(deletedProduct.IsActive);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Act
            var result = await _productService.DeleteProductAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStock_ShouldUpdateProductStock()
        {
            // Act
            var result = await _productService.UpdateStockAsync(1, -10);

            // Assert
            Assert.True(result);
            
            var product = await _context.Products.FindAsync(1);
            Assert.NotNull(product);
            Assert.Equal(40, product.StockQuantity);
        }

        [Fact]
        public async Task UpdateStock_ShouldPreventNegativeStock()
        {
            // Act
            var result = await _productService.UpdateStockAsync(1, -100);

            // Assert
            Assert.True(result);
            
            var product = await _context.Products.FindAsync(1);
            Assert.NotNull(product);
            Assert.Equal(0, product.StockQuantity);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnAllActiveCategories()
        {
            // Act
            var categories = await _productService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count());
            Assert.All(categories, c => Assert.True(c.IsActive));
        }

        [Fact]
        public async Task CreateCategory_ShouldCreateNewCategory()
        {
            // Arrange
            var createCategoryDto = new CreateProductCategoryDto
            {
                Name = "Accessories",
                Description = "Gaming accessories and peripherals",
                IconUrl = "/icons/accessories.png",
                SortOrder = 3
            };

            // Act
            var category = await _productService.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Accessories", category.Name);
            Assert.Equal("Gaming accessories and peripherals", category.Description);
            Assert.Equal(3, category.SortOrder);
            Assert.True(category.IsActive);
        }

        [Fact]
        public async Task UpdateCategory_ShouldUpdateExistingCategory()
        {
            // Arrange
            var updateCategoryDto = new UpdateProductCategoryDto
            {
                Name = "Updated Electronics",
                SortOrder = 5
            };

            // Act
            var category = await _productService.UpdateCategoryAsync(1, updateCategoryDto);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Updated Electronics", category.Name);
            Assert.Equal(5, category.SortOrder);
            Assert.NotNull(category.UpdatedAt);
        }

        [Fact]
        public async Task DeleteCategory_ShouldDeactivateCategory()
        {
            // Act
            var result = await _productService.DeleteCategoryAsync(1);

            // Assert
            Assert.True(result);
            
            var deletedCategory = await _context.ProductCategories.FindAsync(1);
            Assert.NotNull(deletedCategory);
            Assert.False(deletedCategory.IsActive);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}