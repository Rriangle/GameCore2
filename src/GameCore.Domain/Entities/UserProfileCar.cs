using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Car")]
public class UserProfileCar
{
    [Key]
    public int CarID { get; set; }
    public int UserID { get; set; }
    public string CarModel { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 