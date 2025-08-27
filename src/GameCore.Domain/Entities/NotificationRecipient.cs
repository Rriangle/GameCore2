using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知接收者實體
    /// </summary>
    [Table("Notification_Recipients")]
    public class NotificationRecipient
    {
        [Key]
        public int recipient_id { get; set; }
        
        [Required]
        public int notification_id { get; set; }
        
        [Required]
        public int user_id { get; set; }
        
        [Required]
        public bool is_read { get; set; } = false;
        
        public DateTime? read_at { get; set; }

        // Navigation properties
        public virtual Notification Notification { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}