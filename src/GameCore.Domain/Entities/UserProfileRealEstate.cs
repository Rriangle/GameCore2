using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_RealEstate")]
public class UserProfileRealEstate
{
    [Key]
    public int RealEstateID { get; set; }
    public int UserID { get; set; }
    public string RealEstateType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 