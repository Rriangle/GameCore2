using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

public class AdminAction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AdminId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // User, Content, System, Security

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Details { get; set; }

    [MaxLength(50)]
    public string? TargetType { get; set; } // User, Post, Thread, etc.

    public int? TargetId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Completed"; // Completed, Failed, Pending

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    [Required]
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("AdminId")]
    public virtual Admin Admin { get; set; } = null!;
}