using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知表
    /// </summary>
    [Table("notifications")]
    public class Notification
    {
        /// <summary>
        /// 通知ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long notification_id { get; set; }

        /// <summary>
        /// 發送者ID（外鍵參考 users.user_id）
        /// </summary>
        public int? sender_id { get; set; }

        /// <summary>
        /// 通知類型（system/user/order/forum）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string type { get; set; } = "system";

        /// <summary>
        /// 通知標題
        /// </summary>
        [Required]
        [StringLength(200)]
        public string title { get; set; } = string.Empty;

        /// <summary>
        /// 通知內容
        /// </summary>
        [Required]
        public string content { get; set; } = string.Empty;

        /// <summary>
        /// 通知狀態（unread/read/archived）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "unread";

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 讀取時間
        /// </summary>
        public DateTime? read_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 發送者
        /// </summary>
        [ForeignKey("sender_id")]
        public virtual User? Sender { get; set; }

        /// <summary>
        /// 通知接收者
        /// </summary>
        public virtual ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
    }
}