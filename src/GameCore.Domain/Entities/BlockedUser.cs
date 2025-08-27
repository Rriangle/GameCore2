using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("BlockedUser")]
public class BlockedUser
{
    [Key]
    public int BlockId { get; set; }

    [Required]
    public int BlockerId { get; set; }

    [Required]
    public int BlockedId { get; set; }

    [StringLength(200)]
    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("BlockerId")]
    public virtual User Blocker { get; set; } = null!;

    [ForeignKey("BlockedId")]
    public virtual User Blocked { get; set; } = null!;
}