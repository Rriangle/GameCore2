using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Religion")]
public class UserProfileReligion
{
    [Key]
    public int ReligionID { get; set; }
    public int UserID { get; set; }
    public string ReligionName { get; set; } = string.Empty;
    public string? Denomination { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 