using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Friend")]
public class UserFriend
{
    public int UserID { get; set; }
    public int FriendID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 