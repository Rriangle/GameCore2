using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 統一貼文表：洞察與未來UGC都走這
    /// </summary>
    [Table("posts")]
    public class Post
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Key]
        public int post_id { get; set; }
        
        /// <summary>
        /// 類型（insight/user）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string type { get; set; } = string.Empty;
        
        /// <summary>
        /// 關聯遊戲（可為NULL）
        /// </summary>
        public int? game_id { get; set; }
        
        /// <summary>
        /// 標題
        /// </summary>
        [Required]
        [StringLength(500)]
        public string title { get; set; } = string.Empty;
        
        /// <summary>
        /// 三行摘要（卡片用）
        /// </summary>
        [StringLength(300)]
        public string tldr { get; set; } = string.Empty;
        
        /// <summary>
        /// 內文（Markdown）
        /// </summary>
        [StringLength(10000)]
        public string body_md { get; set; } = string.Empty;
        
        /// <summary>
        /// 公開/隱藏
        /// </summary>
        [Required]
        public bool visibility { get; set; } = true;
        
        /// <summary>
        /// 狀態
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "draft";
        
        /// <summary>
        /// 是否置頂
        /// </summary>
        [Required]
        public bool pinned { get; set; } = false;
        
        /// <summary>
        /// 作者ID
        /// </summary>
        [Required]
        public int created_by { get; set; }
        
        /// <summary>
        /// 發佈時間
        /// </summary>
        public DateTime? published_at { get; set; }
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }
        
        /// <summary>
        /// 導航屬性：關聯遊戲
        /// </summary>
        public virtual Game? Game { get; set; }
        
        /// <summary>
        /// 導航屬性：作者
        /// </summary>
        public virtual User CreatedByUser { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：指標快照
        /// </summary>
        public virtual PostMetricSnapshot? PostMetricSnapshot { get; set; }
        
        /// <summary>
        /// 導航屬性：引用來源
        /// </summary>
        public virtual ICollection<PostSource> PostSources { get; set; } = new List<PostSource>();
    }
}