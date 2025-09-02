using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Medical")]
public class UserProfileMedical
{
    [Key]
    public int MedicalID { get; set; }
    public int UserID { get; set; }
    public string MedicalField { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 