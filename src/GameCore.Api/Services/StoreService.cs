using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class StoreService : IStoreService
{
    private readonly GameCoreDbContext _context;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<StoreService> _logger;

    public StoreService(GameCoreDbContext context, IUserWalletRepository walletRepository, ILogger<StoreService> logger)
    {
        _context = context;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<StoreProductDto>> GetProductsAsync(string? category = null)
    {
        try
        {
            var query = _context.StoreProducts
                .Include(p => p.Supplier)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var products = await query.ToListAsync();

            return products.Select(p => new StoreProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Category = p.Category,
                ImageUrl = p.ImageUrl,
                SupplierName = p.Supplier.Name
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store products");
            return Enumerable.Empty<StoreProductDto>();
        }
    }

    public async Task<StoreProductDto?> GetProductAsync(int productId)
    {
        try
        {
            var product = await _context.StoreProducts
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null) return null;

            return new StoreProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                ImageUrl = product.ImageUrl,
                SupplierName = product.Supplier.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store product {ProductId}", productId);
            return null;
        }
    }

    public async Task<OrderResult> CreateOrderAsync(int userId, int productId, int quantity, string? shippingAddress)
    {
        try
        {
            var product = await _context.StoreProducts
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "商品不存在"
                };
            }

            if (product.Stock < quantity)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "庫存不足"
                };
            }

            // Check if user has enough points
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "用戶錢包不存在"
                };
            }

            var totalAmount = product.Price * quantity;
            if (wallet.Balance < totalAmount)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "點數不足"
                };
            }

            // Create order
            var order = new OrderInfo
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                TotalAmount = totalAmount,
                OrderStatus = "Created",
                PaymentStatus = "Placed",
                ShippingAddress = shippingAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OrderInfos.Add(order);

            // Update stock
            product.Stock -= quantity;
            product.UpdatedAt = DateTime.UtcNow;

            // Deduct points
            wallet.Balance -= totalAmount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Order created: User {UserId}, Product {ProductId}, Quantity {Quantity}, Amount {Amount}", 
                userId, productId, quantity, totalAmount);

            return new OrderResult
            {
                Success = true,
                Message = "訂單創建成功",
                Order = new OrderDto
                {
                    OrderId = order.OrderId,
                    ProductId = order.ProductId,
                    ProductName = product.Name,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    ShippingAddress = order.ShippingAddress,
                    CreatedAt = order.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
            return new OrderResult
            {
                Success = false,
                Message = "創建訂單失敗"
            };
        }
    }

    public async Task<OrderResult> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        try
        {
            var order = await _context.OrderInfos
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "訂單不存在"
                };
            }

            // Validate status transition
            if (!IsValidStatusTransition(order.OrderStatus, newStatus))
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "無效的狀態變更"
                };
            }

            order.OrderStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Set timestamps based on status
            switch (newStatus)
            {
                case "Shipped":
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case "Completed":
                    order.CompletedAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} status updated to {NewStatus}", orderId, newStatus);

            return new OrderResult
            {
                Success = true,
                Message = "訂單狀態更新成功",
                Order = await MapToOrderDto(order)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
            return new OrderResult
            {
                Success = false,
                Message = "更新訂單狀態失敗"
            };
        }
    }

    public async Task<OrderResult> UpdatePaymentStatusAsync(int orderId, string newStatus)
    {
        try
        {
            var order = await _context.OrderInfos
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return new OrderResult
                {
                    Success = false,
                    Message = "訂單不存在"
                };
            }

            order.PaymentStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == "Paid")
            {
                order.PaymentAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} payment status updated to {NewStatus}", orderId, newStatus);

            return new OrderResult
            {
                Success = true,
                Message = "付款狀態更新成功",
                Order = await MapToOrderDto(order)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for order {OrderId}", orderId);
            return new OrderResult
            {
                Success = false,
                Message = "更新付款狀態失敗"
            };
        }
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
    {
        try
        {
            var orders = await _context.OrderInfos
                .Include(o => o.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                orderDtos.Add(await MapToOrderDto(order));
            }

            return orderDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for user {UserId}", userId);
            return Enumerable.Empty<OrderDto>();
        }
    }

    public async Task<OrderDto?> GetOrderAsync(int orderId)
    {
        try
        {
            var order = await _context.OrderInfos
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            return order != null ? await MapToOrderDto(order) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", orderId);
            return null;
        }
    }

    private async Task<OrderDto> MapToOrderDto(OrderInfo order)
    {
        var product = await _context.StoreProducts
            .FirstOrDefaultAsync(p => p.ProductId == order.ProductId);

        return new OrderDto
        {
            OrderId = order.OrderId,
            ProductId = order.ProductId,
            ProductName = product?.Name ?? "未知商品",
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            PaymentStatus = order.PaymentStatus,
            ShippingAddress = order.ShippingAddress,
            TrackingNumber = order.TrackingNumber,
            PaymentAt = order.PaymentAt,
            ShippedAt = order.ShippedAt,
            CompletedAt = order.CompletedAt,
            CreatedAt = order.CreatedAt
        };
    }

    private bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        return currentStatus switch
        {
            "Created" => newStatus == "ToShip" || newStatus == "Cancelled",
            "ToShip" => newStatus == "Shipped" || newStatus == "Cancelled",
            "Shipped" => newStatus == "Completed",
            _ => false
        };
    }
}