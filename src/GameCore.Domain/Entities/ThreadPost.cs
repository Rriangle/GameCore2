using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 論壇主題回覆表
    /// </summary>
    [Table("thread_posts")]
    public class ThreadPost
    {
        /// <summary>
        /// 回覆ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long post_id { get; set; }

        /// <summary>
        /// 主題ID（外鍵參考 threads.thread_id）
        /// </summary>
        [Required]
        public long thread_id { get; set; }

        /// <summary>
        /// 作者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int author_id { get; set; }

        /// <summary>
        /// 回覆內容
        /// </summary>
        [Required]
        public string content { get; set; } = string.Empty;

        /// <summary>
        /// 回覆狀態（active/hidden/deleted）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 點讚次數
        /// </summary>
        public int like_count { get; set; } = 0;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 主題
        /// </summary>
        [ForeignKey("thread_id")]
        public virtual Thread Thread { get; set; } = null!;

        /// <summary>
        /// 作者
        /// </summary>
        [ForeignKey("author_id")]
        public virtual User Author { get; set; } = null!;
    }
}