using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Pet Achievement entity for tracking pet achievements and milestones
/// </summary>
[Table("PetAchievements")]
public class PetAchievement
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int PetId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; }
    
    [Required]
    public int PointsReward { get; set; }
    
    [Required]
    public bool IsUnlocked { get; set; }
    
    [Required]
    public DateTime? UnlockedAt { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PetId")]
    public virtual VirtualPet Pet { get; set; }
}