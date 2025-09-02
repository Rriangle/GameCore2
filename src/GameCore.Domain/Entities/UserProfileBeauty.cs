using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Beauty")]
public class UserProfileBeauty
{
    [Key]
    public int BeautyID { get; set; }
    public int UserID { get; set; }
    public string BeautyCategory { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 