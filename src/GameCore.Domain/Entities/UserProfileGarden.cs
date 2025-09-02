using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Garden")]
public class UserProfileGarden
{
    [Key]
    public int GardenID { get; set; }
    public int UserID { get; set; }
    public string? GardenType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 