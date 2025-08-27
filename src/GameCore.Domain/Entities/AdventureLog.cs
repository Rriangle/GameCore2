using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

public class AdventureLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AdventureId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "InProgress"; // InProgress, Completed, Failed, Abandoned

    [Required]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime? FailedAt { get; set; }

    [Required]
    public int EnergySpent { get; set; }

    [Required]
    public int ExperienceGained { get; set; } = 0;

    [Required]
    public int PointsGained { get; set; } = 0;

    [Required]
    public decimal GoldGained { get; set; } = 0.00m;

    [Required]
    public int HealthChange { get; set; } = 0;

    [Required]
    public int HungerChange { get; set; } = 0;

    [Required]
    public int EnergyChange { get; set; } = 0;

    [Required]
    public int HappinessChange { get; set; } = 0;

    [Required]
    public int CleanlinessChange { get; set; } = 0;

    [MaxLength(1000)]
    public string? AdventureNotes { get; set; }

    [MaxLength(500)]
    public string? FailureReason { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("AdventureId")]
    public virtual Adventure Adventure { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<MonsterEncounter> MonsterEncounters { get; set; } = new List<MonsterEncounter>();
}