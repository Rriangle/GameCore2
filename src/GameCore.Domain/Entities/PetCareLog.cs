using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Pet Care Log entity for tracking pet care activities
/// </summary>
[Table("PetCareLogs")]
public class PetCareLog
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int PetId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Action { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    public int HealthChange { get; set; }
    
    [Required]
    public int HungerChange { get; set; }
    
    [Required]
    public int EnergyChange { get; set; }
    
    [Required]
    public int HappinessChange { get; set; }
    
    [Required]
    public int CleanlinessChange { get; set; }
    
    [Required]
    public int ExperienceGained { get; set; }
    
    [Required]
    public int PointsEarned { get; set; }
    
    [Required]
    public DateTime ActionTime { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PetId")]
    public virtual VirtualPet Pet { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}