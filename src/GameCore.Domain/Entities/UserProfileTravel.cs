using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Travel")]
public class UserProfileTravel
{
    [Key]
    public int TravelID { get; set; }
    public int UserID { get; set; }
    public string Destination { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? VisitDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 