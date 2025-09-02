using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Hobby")]
public class UserProfileHobby
{
    [Key]
    public int HobbyID { get; set; }
    public int UserID { get; set; }
    public string HobbyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 