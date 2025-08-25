using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly GameCoreDbContext _context;

        public OrderService(GameCoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Calculate order totals
            var totalAmount = await CalculateOrderTotalAsync(createOrderDto);
            var taxAmount = totalAmount * 0.1m; // 10% tax
            var shippingAmount = totalAmount > 100 ? 0 : 10; // Free shipping over $100
            var finalAmount = totalAmount + taxAmount + shippingAmount;

            // Generate order number
            var orderNumber = GenerateOrderNumber();

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = createOrderDto.UserId,
                TotalAmount = totalAmount,
                TaxAmount = taxAmount,
                ShippingAmount = shippingAmount,
                FinalAmount = finalAmount,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = createOrderDto.ShippingAddress,
                ShippingCity = createOrderDto.ShippingCity,
                ShippingPostalCode = createOrderDto.ShippingPostalCode,
                ShippingCountry = createOrderDto.ShippingCountry,
                ContactPhone = createOrderDto.ContactPhone,
                Notes = createOrderDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product {itemDto.ProductId} not found");

                if (product.StockQuantity < itemDto.Quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Name}");

                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    ProductName = product.Name,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * itemDto.Quantity,
                    ProductImageUrl = product.ImageUrl,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderItems.Add(orderItem);

                // Update product stock
                product.StockQuantity -= itemDto.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new ArgumentException("Order not found");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            // Update timestamps based on status
            switch (status)
            {
                case OrderStatus.Confirmed:
                    order.UpdatedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // Restore product stock
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order> ProcessPaymentAsync(int orderId, ProcessPaymentDto processPaymentDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.PaymentStatus == PaymentStatus.Paid)
                throw new InvalidOperationException("Order is already paid");

            // Create payment transaction
            var paymentTransaction = new PaymentTransaction
            {
                OrderId = orderId,
                TransactionId = processPaymentDto.TransactionId,
                Amount = order.FinalAmount,
                PaymentMethod = processPaymentDto.PaymentMethod,
                Status = PaymentStatus.Paid,
                Description = processPaymentDto.Description,
                PaymentDetails = processPaymentDto.PaymentDetails,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(paymentTransaction);

            // Update order status
            order.PaymentStatus = PaymentStatus.Paid;
            order.Status = OrderStatus.Confirmed;
            order.PaidAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> ConfirmOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending || order.PaymentStatus != PaymentStatus.Paid)
                return false;

            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ShipOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Confirmed)
                return false;

            order.Status = OrderStatus.Shipped;
            order.ShippedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeliverOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Shipped)
                return false;

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == status)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> CalculateOrderTotalAsync(CreateOrderDto createOrderDto)
        {
            decimal total = 0;
            foreach (var item in createOrderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    total += product.Price * item.Quantity;
                }
            }
            return total;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}