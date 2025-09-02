using System.ComponentModel.DataAnnotations;

namespace GameCore.Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [Required]
        public ProductCategory Category { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // 導航屬性
        public virtual ICollection<MarketItem> MarketItems { get; set; } = new List<MarketItem>();
        public virtual ICollection<MarketTransaction> MarketTransactions { get; set; } = new List<MarketTransaction>();
    }

    public enum ProductCategory
    {
        Games = 1,
        Consoles = 2,
        Accessories = 3,
        Collectibles = 4,
        Digital = 5,
        Other = 6
    }
}
