using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Interest")]
public class UserProfileInterest
{
    [Key]
    public int InterestID { get; set; }
    public int UserID { get; set; }
    public string InterestName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 