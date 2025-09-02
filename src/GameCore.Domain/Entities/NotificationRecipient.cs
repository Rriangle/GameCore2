using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Notification_Recipients")]
public class NotificationRecipient
{
    [Key]
    public int RecipientID { get; set; }
    public int NotificationID { get; set; }
    public int UserID { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
} 