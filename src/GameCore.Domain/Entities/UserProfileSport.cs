using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Sport")]
public class UserProfileSport
{
    [Key]
    public int SportID { get; set; }
    public int UserID { get; set; }
    public string SportName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SkillLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 