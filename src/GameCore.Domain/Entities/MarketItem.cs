using System.ComponentModel.DataAnnotations;

namespace GameCore.Domain.Entities
{
    public class MarketItem
    {
        [Key]
        public int MarketItemId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public MarketItemStatus Status { get; set; } = MarketItemStatus.Active;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 導航屬性
        public virtual User Seller { get; set; } = null!;
    }

    public enum MarketItemStatus
    {
        Active = 1,
        Sold = 2,
        Cancelled = 3,
        Expired = 4
    }
}
