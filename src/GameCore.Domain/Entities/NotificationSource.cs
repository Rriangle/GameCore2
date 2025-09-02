using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Notification_Sources")]
public class NotificationSource
{
    [Key]
    public int SourceID { get; set; }
    public string SourceName { get; set; } = string.Empty;
} 