using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通用收藏表（多型）
    /// </summary>
    [Table("bookmarks")]
    public class Bookmark
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        public long id { get; set; }
        
        /// <summary>
        /// 收藏者ID
        /// </summary>
        [Required]
        public int user_id { get; set; }
        
        /// <summary>
        /// 目標類型'post' | 'thread' | 'game' | 'forum'
        /// </summary>
        [Required]
        [StringLength(20)]
        public string target_type { get; set; } = string.Empty;
        
        /// <summary>
        /// 目標ID（多型，不設FK）
        /// </summary>
        [Required]
        public long target_id { get; set; }
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：使用者
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}