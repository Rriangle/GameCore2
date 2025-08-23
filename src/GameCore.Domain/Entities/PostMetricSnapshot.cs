using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 貼文指標快照表
    /// </summary>
    [Table("post_metric_snapshots")]
    public class PostMetricSnapshot
    {
        /// <summary>
        /// 快照ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long snapshot_id { get; set; }

        /// <summary>
        /// 貼文ID（外鍵參考 posts.post_id）
        /// </summary>
        [Required]
        public long post_id { get; set; }

        /// <summary>
        /// 快照時間
        /// </summary>
        [Required]
        public DateTime snapshot_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 瀏覽次數
        /// </summary>
        public int view_count { get; set; } = 0;

        /// <summary>
        /// 點讚次數
        /// </summary>
        public int like_count { get; set; } = 0;

        /// <summary>
        /// 收藏次數
        /// </summary>
        public int bookmark_count { get; set; } = 0;

        /// <summary>
        /// 評論次數
        /// </summary>
        public int comment_count { get; set; } = 0;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 貼文
        /// </summary>
        [ForeignKey("post_id")]
        public virtual Post Post { get; set; } = null!;
    }
}