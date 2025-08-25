using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 版內主題（討論串）
    /// </summary>
    [Table("threads")]
    public class Thread
    {
        /// <summary>
        /// 主題ID
        /// </summary>
        [Key]
        public long thread_id { get; set; }
        
        /// <summary>
        /// 所屬版ID
        /// </summary>
        [Required]
        public int forum_id { get; set; }
        
        /// <summary>
        /// 作者ID
        /// </summary>
        [Required]
        public int author_user_id { get; set; }
        
        /// <summary>
        /// 標題
        /// </summary>
        [Required]
        [StringLength(500)]
        public string title { get; set; } = string.Empty;
        
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
        /// 導航屬性：所屬論壇
        /// </summary>
        public virtual Forum Forum { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：作者
        /// </summary>
        public virtual User AuthorUser { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：回覆
        /// </summary>
        public virtual ICollection<ThreadPost> ThreadPosts { get; set; } = new List<ThreadPost>();
    }
}