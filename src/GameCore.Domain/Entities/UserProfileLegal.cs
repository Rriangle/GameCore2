using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Legal")]
public class UserProfileLegal
{
    [Key]
    public int LegalID { get; set; }
    public int UserID { get; set; }
    public string LegalField { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 