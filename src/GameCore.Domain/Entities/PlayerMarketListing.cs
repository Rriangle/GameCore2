using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    [Table("PlayerMarketListings")]
    public class PlayerMarketListing
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SellerId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public int AvailableQuantity { get; set; }
        
        [Required]
        public ListingStatus Status { get; set; } = ListingStatus.Active;
        
        [StringLength(100)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        public bool IsNegotiable { get; set; } = false;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public DateTime? SoldAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }
        
        // Navigation properties
        public virtual User Seller { get; set; } = null!;
        public virtual ICollection<PlayerMarketOrder> Orders { get; set; } = new List<PlayerMarketOrder>();
    }
    
    public enum ListingStatus
    {
        Active = 0,
        Sold = 1,
        Cancelled = 2,
        Expired = 3,
        PendingApproval = 4
    }
}