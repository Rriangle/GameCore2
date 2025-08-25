using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 論壇版主檔：每遊戲一個版
    /// </summary>
    [Table("forums")]
    public class Forum
    {
        /// <summary>
        /// 論壇版ID
        /// </summary>
        [Key]
        public int forum_id { get; set; }
        
        /// <summary>
        /// 遊戲ID（一對一）
        /// </summary>
        [Required]
        public int game_id { get; set; }
        
        /// <summary>
        /// 版名
        /// </summary>
        [Required]
        [StringLength(200)]
        public string name { get; set; } = string.Empty;
        
        /// <summary>
        /// 版說明
        /// </summary>
        [StringLength(1000)]
        public string description { get; set; } = string.Empty;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：主題
        /// </summary>
        public virtual ICollection<Thread> Threads { get; set; } = new List<Thread>();
    }
}