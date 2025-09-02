using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int productId);
        Task<List<Product>> GetAllAsync();
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int productId);
        Task SaveChangesAsync();
    }
}
