using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Language")]
public class UserProfileLanguage
{
    [Key]
    public int LanguageID { get; set; }
    public int UserID { get; set; }
    public string LanguageName { get; set; } = string.Empty;
    public string? ProficiencyLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 