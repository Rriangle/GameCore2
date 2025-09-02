using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Derivative")]
public class UserProfileDerivative
{
    [Key]
    public int DerivativeID { get; set; }
    public int UserID { get; set; }
    public string DerivativeType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 