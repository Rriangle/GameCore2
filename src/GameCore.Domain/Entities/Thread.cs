using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 論壇主題表
    /// </summary>
    [Table("threads")]
    public class Thread
    {
        /// <summary>
        /// 主題ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long thread_id { get; set; }

        /// <summary>
        /// 論壇ID（外鍵參考 forums.forum_id）
        /// </summary>
        [Required]
        public int forum_id { get; set; }

        /// <summary>
        /// 作者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int author_id { get; set; }

        /// <summary>
        /// 主題標題
        /// </summary>
        [Required]
        [StringLength(200)]
        public string title { get; set; } = string.Empty;

        /// <summary>
        /// 主題內容
        /// </summary>
        [Required]
        public string content { get; set; } = string.Empty;

        /// <summary>
        /// 主題狀態（active/pinned/locked/deleted）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 瀏覽次數
        /// </summary>
        public int view_count { get; set; } = 0;

        /// <summary>
        /// 回覆次數
        /// </summary>
        public int reply_count { get; set; } = 0;

        /// <summary>
        /// 最後回覆時間
        /// </summary>
        public DateTime? last_reply_at { get; set; }

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
        /// 論壇
        /// </summary>
        [ForeignKey("forum_id")]
        public virtual Forum Forum { get; set; } = null!;

        /// <summary>
        /// 作者
        /// </summary>
        [ForeignKey("author_id")]
        public virtual User Author { get; set; } = null!;

        /// <summary>
        /// 主題回覆
        /// </summary>
        public virtual ICollection<ThreadPost> ThreadPosts { get; set; } = new List<ThreadPost>();
    }
}