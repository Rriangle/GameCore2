using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Philosophy")]
public class UserProfilePhilosophy
{
    [Key]
    public int PhilosophyID { get; set; }
    public int UserID { get; set; }
    public string PhilosophyType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 