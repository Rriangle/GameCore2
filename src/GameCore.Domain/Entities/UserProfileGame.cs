using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Game")]
public class UserProfileGame
{
    [Key]
    public int GameID { get; set; }
    public int UserID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string? Platform { get; set; }
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 