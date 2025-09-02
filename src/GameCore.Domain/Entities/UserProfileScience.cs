using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Science")]
public class UserProfileScience
{
    [Key]
    public int ScienceID { get; set; }
    public int UserID { get; set; }
    public string ScienceField { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 