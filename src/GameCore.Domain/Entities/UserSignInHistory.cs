using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// User Sign-In History entity for tracking detailed sign-in history and statistics
/// </summary>
[Table("UserSignInHistories")]
public class UserSignInHistory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [Column(TypeName = "date")]
    public DateTime SignInDate { get; set; }
    
    [Required]
    public DateTime SignInTime { get; set; }
    
    [Required]
    public int DayOfWeek { get; set; }
    
    [Required]
    public int DayOfMonth { get; set; }
    
    [Required]
    public int Month { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    public int WeekOfYear { get; set; }
    
    [Required]
    public int PointsEarned { get; set; }
    
    [Required]
    public bool IsStreakContinued { get; set; }
    
    [Required]
    public bool IsBonusDay { get; set; }
    
    [Required]
    public int BonusMultiplier { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}