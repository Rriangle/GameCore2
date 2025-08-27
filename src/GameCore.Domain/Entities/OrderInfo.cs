using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("OrderInfo")]
public class OrderInfo
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(20)]
    public string OrderStatus { get; set; } = "Created"; // Created, ToShip, Shipped, Completed

    [Required]
    [StringLength(20)]
    public string PaymentStatus { get; set; } = "Placed"; // Placed, Pending, Paid

    [StringLength(200)]
    public string? ShippingAddress { get; set; }

    [StringLength(50)]
    public string? TrackingNumber { get; set; }

    public DateTime? PaymentAt { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ProductId")]
    public virtual StoreProduct Product { get; set; } = null!;
}