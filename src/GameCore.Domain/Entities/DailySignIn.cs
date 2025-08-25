using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// Daily Sign-In entity for tracking user daily sign-ins, streaks, and attendance
/// </summary>
[Table("DailySignIns")]
public class DailySignIn
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
    public int CurrentStreak { get; set; }
    
    [Required]
    public int LongestStreak { get; set; }
    
    [Required]
    public int MonthlyPerfectAttendance { get; set; }
    
    [Required]
    public int PointsEarned { get; set; }
    
    [Required]
    public bool IsBonusDay { get; set; }
    
    [Required]
    public int BonusMultiplier { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}