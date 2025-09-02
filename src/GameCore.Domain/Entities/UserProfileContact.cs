using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Contact")]
public class UserProfileContact
{
    [Key]
    public int ContactID { get; set; }
    public int UserID { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Skype { get; set; }
    public string? WhatsApp { get; set; }
    public string? Telegram { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 