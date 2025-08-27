using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(GameCoreDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.MarketItems)
                    .Include(p => p.MarketTransactions)
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取商品時發生錯誤: {ProductId}", productId);
                throw;
            }
        }

        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.MarketItems)
                    .Include(p => p.MarketTransactions)
                    .Where(p => p.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有商品時發生錯誤");
                throw;
            }
        }

        public async Task<Product> AddAsync(Product product)
        {
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                product.CreatedAt = DateTime.UtcNow;
                product.IsActive = true;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("新增商品成功: {ProductId}, {Name}", product.ProductId, product.Name);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增商品時發生錯誤: {Name}", product?.Name);
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));

                product.UpdatedAt = DateTime.UtcNow;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("更新商品成功: {ProductId}, {Name}", product.ProductId, product.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新商品時發生錯誤: {ProductId}", product?.ProductId);
                throw;
            }
        }

        public async Task DeleteAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("嘗試刪除不存在的商品: {ProductId}", productId);
                    return;
                }

                // 軟刪除
                product.IsActive = false;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("刪除商品成功: {ProductId}, {Name}", product.ProductId, product.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除商品時發生錯誤: {ProductId}", productId);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存變更時發生錯誤");
                throw;
            }
        }
    }
} 