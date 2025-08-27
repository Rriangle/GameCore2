using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組聊天訊息表
    /// </summary>
    [Table("group_chat")]
    public class GroupChat
    {
        /// <summary>
        /// 訊息ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long message_id { get; set; }

        /// <summary>
        /// 群組ID（外鍵參考 groups.group_id）
        /// </summary>
        [Required]
        public int group_id { get; set; }

        /// <summary>
        /// 發送者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int sender_id { get; set; }

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
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 群組
        /// </summary>
        [ForeignKey("group_id")]
        public virtual Group Group { get; set; } = null!;

        /// <summary>
        /// 發送者
        /// </summary>
        [ForeignKey("sender_id")]
        public virtual User Sender { get; set; } = null!;
    }
}