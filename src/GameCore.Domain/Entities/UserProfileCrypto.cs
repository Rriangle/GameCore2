using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Crypto")]
public class UserProfileCrypto
{
    [Key]
    public int CryptoID { get; set; }
    public int UserID { get; set; }
    public string Cryptocurrency { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 