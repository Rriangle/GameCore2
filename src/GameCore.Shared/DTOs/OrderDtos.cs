using System.ComponentModel.DataAnnotations;
using GameCore.Domain.Entities;

namespace GameCore.Shared.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
        
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string ShippingPostalCode { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ShippingCountry { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ContactPhone { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Notes { get; set; } = string.Empty;
    }
    
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
    
    public class UpdateOrderDto
    {
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingPostalCode { get; set; }
        public string? ShippingCountry { get; set; }
        public string? ContactPhone { get; set; }
        public string? Notes { get; set; }
    }
    
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingPostalCode { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
    }
    
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductImageUrl { get; set; } = string.Empty;
    }
    
    public class ProcessPaymentDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string PaymentDetails { get; set; } = string.Empty;
    }
    
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? FailedAt { get; set; }
    }
}