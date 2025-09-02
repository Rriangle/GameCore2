using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Video")]
public class UserProfileVideo
{
    [Key]
    public int VideoID { get; set; }
    public int UserID { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 