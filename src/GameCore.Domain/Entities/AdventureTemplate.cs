using System.ComponentModel.DataAnnotations;

namespace GameCore.Domain.Entities;

public class AdventureTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = "Exploration"; // Exploration, Combat, Puzzle, Social

    [Required]
    [MaxLength(50)]
    public string Difficulty { get; set; } = "Normal"; // Easy, Normal, Hard, Extreme

    [Required]
    public int MinLevel { get; set; } = 1;

    [Required]
    public int MaxLevel { get; set; } = 100;

    [Required]
    public int EnergyCost { get; set; } = 10;

    [Required]
    public int DurationMinutes { get; set; } = 30;

    [Required]
    public decimal BaseSuccessRate { get; set; } = 0.7m;

    [Required]
    public int BaseExperienceReward { get; set; } = 50;

    [Required]
    public int BasePointsReward { get; set; } = 25;

    [Required]
    public decimal BaseGoldReward { get; set; } = 10.00m;

    [Required]
    public int MaxMonsterEncounters { get; set; } = 3;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Adventure> Adventures { get; set; } = new List<Adventure>();
}