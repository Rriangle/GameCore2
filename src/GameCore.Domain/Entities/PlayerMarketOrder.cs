using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    [Table("PlayerMarketOrders")]
    public class PlayerMarketOrder
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ListingId { get; set; }
        
        [Required]
        public int BuyerId { get; set; }
        
        [Required]
        public int SellerId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PlatformFee { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellerAmount { get; set; }
        
        [Required]
        public PlayerMarketOrderStatus Status { get; set; } = PlayerMarketOrderStatus.Pending;
        
        [StringLength(500)]
        public string BuyerNotes { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string SellerNotes { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }
        
        public DateTime? DisputedAt { get; set; }
        
        // Navigation properties
        public virtual PlayerMarketListing Listing { get; set; } = null!;
        public virtual User Buyer { get; set; } = null!;
        public virtual User Seller { get; set; } = null!;
    }
    
    public enum PlayerMarketOrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4,
        Disputed = 5,
        Refunded = 6
    }
}