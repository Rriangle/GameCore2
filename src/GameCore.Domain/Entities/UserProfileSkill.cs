using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Skill")]
public class UserProfileSkill
{
    [Key]
    public int SkillID { get; set; }
    public int UserID { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProficiencyLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 