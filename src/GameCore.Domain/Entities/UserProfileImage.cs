using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Image")]
public class UserProfileImage
{
    [Key]
    public int ImageID { get; set; }
    public int UserID { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsProfilePicture { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 