using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Block")]
public class UserBlock
{
    [Key]
    public int BlockID { get; set; }
    public int BlockerID { get; set; }
    public int BlockedUserID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 