using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Technology")]
public class UserProfileTechnology
{
    [Key]
    public int TechnologyID { get; set; }
    public int UserID { get; set; }
    public string TechnologyField { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 