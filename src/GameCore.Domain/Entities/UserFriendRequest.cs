using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Friend_Request")]
public class UserFriendRequest
{
    [Key]
    public int RequestID { get; set; }
    public int RequesterID { get; set; }
    public int RequestedUserID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }
} 