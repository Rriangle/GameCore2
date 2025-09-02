using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 商品 DTO
/// </summary>
public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int SupplierId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 商品詳細資訊 DTO
/// </summary>
public class ProductDetailDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 訂單 DTO
/// </summary>
public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

/// <summary>
/// 訂單項目 DTO
/// </summary>
public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// 建立訂單請求 DTO
/// </summary>
public class CreateOrderRequest
{
    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();
}

/// <summary>
/// 建立訂單項目請求 DTO
/// </summary>
public class CreateOrderItemRequest
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, 999)]
    public int Quantity { get; set; }
} 