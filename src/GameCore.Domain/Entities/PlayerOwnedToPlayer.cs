using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("PlayerOwnedToPlayer")]
public class PlayerOwnedToPlayer
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    public int ItemId { get; set; }

    [Required]
    public int SellerId { get; set; }

    [Required]
    public int BuyerId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PlatformFee { get; set; }

    [Required]
    [StringLength(20)]
    public string OrderStatus { get; set; } = "Created"; // Created, Trading, Completed, Cancelled

    [Required]
    [StringLength(20)]
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, N/A

    public DateTime? SellerConfirmedAt { get; set; }

    public DateTime? BuyerConfirmedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ItemId")]
    public virtual PlayerOwned Item { get; set; } = null!;

    [ForeignKey("SellerId")]
    public virtual User Seller { get; set; } = null!;

    [ForeignKey("BuyerId")]
    public virtual User Buyer { get; set; } = null!;
}