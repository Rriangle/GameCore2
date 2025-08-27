using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

public class Adventure
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Difficulty { get; set; } = "Normal"; // Easy, Normal, Hard, Extreme

    [Required]
    public int RequiredLevel { get; set; } = 1;

    [Required]
    public int RequiredEnergy { get; set; } = 10;

    [Required]
    public int DurationMinutes { get; set; } = 30;

    [Required]
    public decimal SuccessRate { get; set; } = 0.7m; // 70% base success rate

    [Required]
    public int BaseExperienceReward { get; set; } = 50;

    [Required]
    public int BasePointsReward { get; set; } = 25;

    [Required]
    public decimal BaseGoldReward { get; set; } = 10.00m;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<AdventureLog> AdventureLogs { get; set; } = new List<AdventureLog>();
}