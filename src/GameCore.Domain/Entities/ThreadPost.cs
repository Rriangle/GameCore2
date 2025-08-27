using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 主題回覆（支援二層結構）
    /// </summary>
    [Table("thread_posts")]
    public class ThreadPost
    {
        /// <summary>
        /// 回覆ID
        /// </summary>
        [Key]
        public long id { get; set; }
        
        /// <summary>
        /// 主題ID
        /// </summary>
        [Required]
        public long thread_id { get; set; }
        
        /// <summary>
        /// 回覆者ID
        /// </summary>
        [Required]
        public int author_user_id { get; set; }
        
        /// <summary>
        /// 內容（Markdown）
        /// </summary>
        [Required]
        [StringLength(5000)]
        public string content_md { get; set; } = string.Empty;
        
        /// <summary>
        /// 父回覆ID（支援二層）
        /// </summary>
        public long? parent_post_id { get; set; }
        
        /// <summary>
        /// 狀態
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "normal";
        
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
        /// 導航屬性：所屬主題
        /// </summary>
        public virtual Thread Thread { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：作者
        /// </summary>
        public virtual User AuthorUser { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：父回覆
        /// </summary>
        public virtual ThreadPost? ParentPost { get; set; }
        
        /// <summary>
        /// 導航屬性：子回覆
        /// </summary>
        public virtual ICollection<ThreadPost> ChildPosts { get; set; } = new List<ThreadPost>();
    }
}