using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Spirituality")]
public class UserProfileSpirituality
{
    [Key]
    public int SpiritualityID { get; set; }
    public int UserID { get; set; }
    public string SpiritualityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 