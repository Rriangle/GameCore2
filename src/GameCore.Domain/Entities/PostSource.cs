using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 洞察引用來源清單
    /// </summary>
    [Table("post_sources")]
    public class PostSource
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        public long id { get; set; }
        
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required]
        public long post_id { get; set; }
        
        /// <summary>
        /// 顯示名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string source_name { get; set; } = string.Empty;
        
        /// <summary>
        /// 外部連結
        /// </summary>
        [StringLength(500)]
        public string url { get; set; } = string.Empty;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：貼文
        /// </summary>
        public virtual Post Post { get; set; } = null!;
    }
}