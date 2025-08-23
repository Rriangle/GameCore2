using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 聊天訊息表
    /// </summary>
    [Table("chat_messages")]
    public class ChatMessage
    {
        /// <summary>
        /// 訊息ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long message_id { get; set; }

        /// <summary>
        /// 發送者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int sender_id { get; set; }

        /// <summary>
        /// 接收者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int receiver_id { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        [Required]
        public string message_content { get; set; } = string.Empty;

        /// <summary>
        /// 訊息類型（text/image/file/system）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string message_type { get; set; } = "text";

        /// <summary>
        /// 訊息狀態（sent/delivered/read）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "sent";

        /// <summary>
        /// 發送時間
        /// </summary>
        public DateTime sent_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 送達時間
        /// </summary>
        public DateTime? delivered_at { get; set; }

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
        /// 發送者
        /// </summary>
        [ForeignKey("sender_id")]
        public virtual User Sender { get; set; } = null!;

        /// <summary>
        /// 接收者
        /// </summary>
        [ForeignKey("receiver_id")]
        public virtual User Receiver { get; set; } = null!;
    }
}