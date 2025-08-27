using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Virtual Pet entity for the slime virtual pet system
/// </summary>
[Table("VirtualPets")]
public class VirtualPet
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    public int Level { get; set; }
    
    [Required]
    public int Experience { get; set; }
    
    [Required]
    public int ExperienceToNextLevel { get; set; }
    
    [Required]
    public int Health { get; set; }
    
    [Required]
    public int MaxHealth { get; set; }
    
    [Required]
    public int Hunger { get; set; }
    
    [Required]
    public int MaxHunger { get; set; }
    
    [Required]
    public int Energy { get; set; }
    
    [Required]
    public int MaxEnergy { get; set; }
    
    [Required]
    public int Happiness { get; set; }
    
    [Required]
    public int MaxHappiness { get; set; }
    
    [Required]
    public int Cleanliness { get; set; }
    
    [Required]
    public int MaxCleanliness { get; set; }
    
    [Required]
    public string Color { get; set; }
    
    [Required]
    public string Personality { get; set; }
    
    [Required]
    public DateTime LastFed { get; set; }
    
    [Required]
    public DateTime LastPlayed { get; set; }
    
    [Required]
    public DateTime LastCleaned { get; set; }
    
    [Required]
    public DateTime LastRested { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    public virtual ICollection<PetCareLog> CareLogs { get; set; } = new List<PetCareLog>();
    
    public virtual ICollection<PetAchievement> Achievements { get; set; } = new List<PetAchievement>();
}