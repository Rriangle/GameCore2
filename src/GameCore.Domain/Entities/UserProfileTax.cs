using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Tax")]
public class UserProfileTax
{
    [Key]
    public int TaxID { get; set; }
    public int UserID { get; set; }
    public string TaxCategory { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 