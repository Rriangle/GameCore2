using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Forex")]
public class UserProfileForex
{
    [Key]
    public int ForexID { get; set; }
    public int UserID { get; set; }
    public string CurrencyPair { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 