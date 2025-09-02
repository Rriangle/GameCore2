using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Notification_Actions")]
public class NotificationAction
{
    [Key]
    public int ActionID { get; set; }
    public string ActionName { get; set; } = string.Empty;
} 