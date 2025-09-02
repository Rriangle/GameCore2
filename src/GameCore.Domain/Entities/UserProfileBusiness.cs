using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Business")]
public class UserProfileBusiness
{
    [Key]
    public int BusinessID { get; set; }
    public int UserID { get; set; }
    public string BusinessType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 