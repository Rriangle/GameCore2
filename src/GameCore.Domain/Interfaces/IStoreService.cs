namespace GameCore.Domain.Interfaces;

public interface IStoreService
{
    Task<IEnumerable<StoreProductDto>> GetProductsAsync(string? category = null);
    Task<StoreProductDto?> GetProductAsync(int productId);
    Task<OrderResult> CreateOrderAsync(int userId, int productId, int quantity, string? shippingAddress);
    Task<OrderResult> UpdateOrderStatusAsync(int orderId, string newStatus);
    Task<OrderResult> UpdatePaymentStatusAsync(int orderId, string newStatus);
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
    Task<OrderDto?> GetOrderAsync(int orderId);
}

public class StoreProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}

public class OrderDto
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? PaymentAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public OrderDto? Order { get; set; }
}