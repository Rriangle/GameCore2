using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Insurance")]
public class UserProfileInsurance
{
    [Key]
    public int InsuranceID { get; set; }
    public int UserID { get; set; }
    public string InsuranceType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 