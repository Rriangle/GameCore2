using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知接收者表
    /// </summary>
    [Table("notification_recipients")]
    public class NotificationRecipient
    {
        /// <summary>
        /// 記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long record_id { get; set; }

        /// <summary>
        /// 通知ID（外鍵參考 notifications.notification_id）
        /// </summary>
        [Required]
        public long notification_id { get; set; }

        /// <summary>
        /// 接收者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int recipient_id { get; set; }

        /// <summary>
        /// 接收狀態（unread/read/archived）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "unread";

        /// <summary>
        /// 讀取時間
        /// </summary>
        public DateTime? read_at { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 通知
        /// </summary>
        [ForeignKey("notification_id")]
        public virtual Notification Notification { get; set; } = null!;

        /// <summary>
        /// 接收者
        /// </summary>
        [ForeignKey("recipient_id")]
        public virtual User Recipient { get; set; } = null!;
    }
}