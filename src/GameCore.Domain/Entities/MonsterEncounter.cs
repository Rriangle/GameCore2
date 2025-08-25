using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

public class MonsterEncounter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AdventureLogId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MonsterName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string MonsterType { get; set; } = "Normal"; // Normal, Elite, Boss, MiniBoss

    [Required]
    public int MonsterLevel { get; set; } = 1;

    [Required]
    public int MonsterHealth { get; set; } = 100;

    [Required]
    public int MonsterMaxHealth { get; set; } = 100;

    [Required]
    public int MonsterAttack { get; set; } = 20;

    [Required]
    public int MonsterDefense { get; set; } = 10;

    [Required]
    [MaxLength(50)]
    public string Outcome { get; set; } = "Victory"; // Victory, Defeat, Escaped, Skipped

    [Required]
    public int DamageDealt { get; set; } = 0;

    [Required]
    public int DamageTaken { get; set; } = 0;

    [Required]
    public int ExperienceGained { get; set; } = 0;

    [Required]
    public int PointsGained { get; set; } = 0;

    [Required]
    public decimal GoldGained { get; set; } = 0.00m;

    [MaxLength(500)]
    public string? BattleNotes { get; set; }

    [Required]
    public DateTime EncounterTime { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("AdventureLogId")]
    public virtual AdventureLog AdventureLog { get; set; } = null!;
}