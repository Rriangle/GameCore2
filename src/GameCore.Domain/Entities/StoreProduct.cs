using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("StoreProduct")]
public class StoreProduct
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [Required]
    [StringLength(20)]
    public string Category { get; set; } = string.Empty; // "game" or "non-game"

    [StringLength(200)]
    public string? ImageUrl { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<OrderInfo> Orders { get; set; } = new List<OrderInfo>();
}