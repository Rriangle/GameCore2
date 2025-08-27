using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Sign-In Reward entity for defining different types of sign-in rewards and their conditions
/// </summary>
[Table("SignInRewards")]
public class SignInReward
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
    public int PointsReward { get; set; }
    
    [Required]
    public int StreakRequirement { get; set; }
    
    [Required]
    public int AttendanceRequirement { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
}