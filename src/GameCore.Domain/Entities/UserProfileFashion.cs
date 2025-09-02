using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Fashion")]
public class UserProfileFashion
{
    [Key]
    public int FashionID { get; set; }
    public int UserID { get; set; }
    public string FashionStyle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 