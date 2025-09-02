using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Audio")]
public class UserProfileAudio
{
    [Key]
    public int AudioID { get; set; }
    public int UserID { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Duration { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 