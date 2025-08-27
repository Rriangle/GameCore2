using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知主表實體
    /// </summary>
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int notification_id { get; set; }
        
        [Required]
        public int source_id { get; set; }
        
        [Required]
        public int action_id { get; set; }
        
        [Required]
        public int sender_id { get; set; }
        
        public int? sender_manager_id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string notification_title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string notification_message { get; set; } = string.Empty;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        public int? group_id { get; set; }

        // Navigation properties
        public virtual NotificationSource Source { get; set; } = null!;
        public virtual NotificationAction Action { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
        public virtual ManagerData? SenderManager { get; set; }
        public virtual Group? Group { get; set; }
        public virtual ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
    }
}