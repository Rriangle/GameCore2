using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Commodity")]
public class UserProfileCommodity
{
    [Key]
    public int CommodityID { get; set; }
    public int UserID { get; set; }
    public string CommodityType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 