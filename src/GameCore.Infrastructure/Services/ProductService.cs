using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly GameCoreDbContext _context;

        public ProductService(GameCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetOfficialStoreProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsOfficialStore && p.IsActive)
                .OrderBy(p => p.Category.SortOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                StockQuantity = createProductDto.StockQuantity,
                CategoryId = createProductDto.CategoryId,
                ImageUrl = createProductDto.ImageUrl,
                IsOfficialStore = createProductDto.IsOfficialStore,
                CreatedBy = 1, // TODO: Get from current user context
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (updateProductDto.Name != null)
                product.Name = updateProductDto.Name;
            if (updateProductDto.Description != null)
                product.Description = updateProductDto.Description;
            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;
            if (updateProductDto.StockQuantity.HasValue)
                product.StockQuantity = updateProductDto.StockQuantity.Value;
            if (updateProductDto.CategoryId.HasValue)
                product.CategoryId = updateProductDto.CategoryId.Value;
            if (updateProductDto.ImageUrl != null)
                product.ImageUrl = updateProductDto.ImageUrl;
            if (updateProductDto.IsActive.HasValue)
                product.IsActive = updateProductDto.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.StockQuantity = Math.Max(0, product.StockQuantity + quantity);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = 1; // TODO: Get from current user context

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ProductCategory> CreateCategoryAsync(CreateProductCategoryDto createCategoryDto)
        {
            var category = new ProductCategory
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                IconUrl = createCategoryDto.IconUrl,
                SortOrder = createCategoryDto.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<ProductCategory> UpdateCategoryAsync(int id, UpdateProductCategoryDto updateCategoryDto)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
                throw new ArgumentException("Category not found");

            if (updateCategoryDto.Name != null)
                category.Name = updateCategoryDto.Name;
            if (updateCategoryDto.Description != null)
                category.Description = updateCategoryDto.Description;
            if (updateCategoryDto.IconUrl != null)
                category.IconUrl = updateCategoryDto.IconUrl;
            if (updateCategoryDto.SortOrder.HasValue)
                category.SortOrder = updateCategoryDto.SortOrder.Value;
            if (updateCategoryDto.IsActive.HasValue)
                category.IsActive = updateCategoryDto.IsActive.Value;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
                return false;

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}