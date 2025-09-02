using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Politics")]
public class UserProfilePolitics
{
    [Key]
    public int PoliticsID { get; set; }
    public int UserID { get; set; }
    public string PoliticalView { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 