using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Pet Item entity for pet items like food, toys, and accessories
/// </summary>
[Table("PetItems")]
public class PetItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Type { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; }
    
    [Required]
    public int HealthEffect { get; set; }
    
    [Required]
    public int HungerEffect { get; set; }
    
    [Required]
    public int EnergyEffect { get; set; }
    
    [Required]
    public int HappinessEffect { get; set; }
    
    [Required]
    public int CleanlinessEffect { get; set; }
    
    [Required]
    public int ExperienceEffect { get; set; }
    
    [Required]
    public int Price { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
}