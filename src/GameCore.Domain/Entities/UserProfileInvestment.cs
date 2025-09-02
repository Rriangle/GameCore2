using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Investment")]
public class UserProfileInvestment
{
    [Key]
    public int InvestmentID { get; set; }
    public int UserID { get; set; }
    public string InvestmentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 