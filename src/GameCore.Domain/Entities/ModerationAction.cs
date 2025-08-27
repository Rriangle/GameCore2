using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

public class ModerationAction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AdminId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Warn, Suspend, Ban, Delete, Hide

    [Required]
    [MaxLength(50)]
    public string TargetType { get; set; } = string.Empty; // User, Post, Thread, Comment

    [Required]
    public int TargetId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Details { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Expired, Reversed

    public DateTime? ExpiresAt { get; set; }

    public DateTime? ReversedAt { get; set; }

    public int? ReversedByAdminId { get; set; }

    [MaxLength(500)]
    public string? ReversalReason { get; set; }

    [Required]
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("AdminId")]
    public virtual Admin Admin { get; set; } = null!;

    [ForeignKey("ReversedByAdminId")]
    public virtual Admin? ReversedByAdmin { get; set; }
}