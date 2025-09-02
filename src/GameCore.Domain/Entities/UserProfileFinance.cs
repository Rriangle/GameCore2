using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Finance")]
public class UserProfileFinance
{
    [Key]
    public int FinanceID { get; set; }
    public int UserID { get; set; }
    public string FinanceCategory { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 