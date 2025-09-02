using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Music")]
public class UserProfileMusic
{
    [Key]
    public int MusicID { get; set; }
    public int UserID { get; set; }
    public string MusicName { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 