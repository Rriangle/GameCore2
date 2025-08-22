using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsEmailVerified { get; set; } = false;

    // 導航屬性
    public virtual UserWallet? Wallet { get; set; }
}
