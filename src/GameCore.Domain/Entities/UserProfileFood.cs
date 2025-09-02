using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Food")]
public class UserProfileFood
{
    [Key]
    public int FoodID { get; set; }
    public int UserID { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public string? Cuisine { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 