using System.ComponentModel.DataAnnotations;

namespace GameCore.Domain.Entities;

public class SystemLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Level { get; set; } = "Info"; // Info, Warning, Error, Critical

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // System, Security, Performance, User

    [Required]
    [MaxLength(200)]
    public string Event { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Details { get; set; }

    [MaxLength(100)]
    public string? Source { get; set; }

    public int? UserId { get; set; }

    public int? AdminId { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}