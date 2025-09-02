using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Yoga")]
public class UserProfileYoga
{
    [Key]
    public int YogaID { get; set; }
    public int UserID { get; set; }
    public string YogaStyle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ExperienceLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 