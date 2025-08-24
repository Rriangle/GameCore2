using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("MiniGame")]
public class MiniGame
{
    [Key]
    public int GameId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    [StringLength(50)]
    public string GameType { get; set; } = string.Empty;

    [Required]
    public bool IsWin { get; set; }

    [Required]
    public int Score { get; set; }

    [Required]
    public int PointsEarned { get; set; }

    [Required]
    public int ExpEarned { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PetId")]
    public virtual Pet Pet { get; set; } = null!;
}