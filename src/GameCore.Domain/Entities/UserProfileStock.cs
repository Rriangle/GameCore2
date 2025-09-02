using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Stock")]
public class UserProfileStock
{
    [Key]
    public int StockID { get; set; }
    public int UserID { get; set; }
    public string StockSymbol { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 