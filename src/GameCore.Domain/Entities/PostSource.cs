using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 貼文引用來源表
    /// </summary>
    [Table("post_sources")]
    public class PostSource
    {
        /// <summary>
        /// 引用來源ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long source_id { get; set; }

        /// <summary>
        /// 貼文ID（外鍵參考 posts.post_id）
        /// </summary>
        [Required]
        public long post_id { get; set; }

        /// <summary>
        /// 來源類型（url/article/book等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string source_type { get; set; } = string.Empty;

        /// <summary>
        /// 來源標題
        /// </summary>
        [StringLength(200)]
        public string? source_title { get; set; }

        /// <summary>
        /// 來源URL
        /// </summary>
        [StringLength(500)]
        public string? source_url { get; set; }

        /// <summary>
        /// 來源描述
        /// </summary>
        [StringLength(500)]
        public string? source_description { get; set; }

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