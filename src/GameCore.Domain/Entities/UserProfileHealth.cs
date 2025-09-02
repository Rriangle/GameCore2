using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Health")]
public class UserProfileHealth
{
    [Key]
    public int HealthID { get; set; }
    public int UserID { get; set; }
    public string HealthCategory { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 