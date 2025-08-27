using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("UserSignInStats")]
public class UserSignInStats
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime SignInDate { get; set; }

    [Required]
    public int PointsEarned { get; set; }

    [Required]
    public int ExpEarned { get; set; }

    [Required]
    public int StreakCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}