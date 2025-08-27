using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知行為類型實體
    /// </summary>
    [Table("Notification_Actions")]
    public class NotificationAction
    {
        [Key]
        public int action_id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string action_name { get; set; } = string.Empty;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }

        // Navigation properties
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}