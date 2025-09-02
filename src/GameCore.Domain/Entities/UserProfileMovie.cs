using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Movie")]
public class UserProfileMovie
{
    [Key]
    public int MovieID { get; set; }
    public int UserID { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public string? Director { get; set; }
    public string? Genre { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 