using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetOfficialStoreProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<PaginatedProductsDto> GetProductsPaginatedAsync(int page = 1, int pageSize = 20, int? categoryId = null, string? searchTerm = null, string sortBy = "Name", string sortDirection = "asc");
        Task<Product> CreateProductAsync(CreateProductDto createProductDto);
        Task<Product> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int id, int quantity);
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task<ProductCategory> CreateCategoryAsync(CreateProductCategoryDto createCategoryDto);
        Task<ProductCategory> UpdateCategoryAsync(int id, UpdateProductCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}