using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Link")]
public class UserProfileLink
{
    [Key]
    public int LinkID { get; set; }
    public int UserID { get; set; }
    public string LinkUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? LinkType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 