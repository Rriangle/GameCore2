using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<Order> UpdateOrderStatusAsync(int id, OrderStatus status);
        Task<bool> CancelOrderAsync(int id);
        Task<Order> ProcessPaymentAsync(int orderId, ProcessPaymentDto processPaymentDto);
        Task<bool> ConfirmOrderAsync(int id);
        Task<bool> ShipOrderAsync(int id);
        Task<bool> DeliverOrderAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<decimal> CalculateOrderTotalAsync(CreateOrderDto createOrderDto);
    }
}