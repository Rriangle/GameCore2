using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 洞察發佈時的數據快照（固定展示）
    /// </summary>
    [Table("post_metric_snapshot")]
    public class PostMetricSnapshot
    {
        /// <summary>
        /// 文章ID（主鍵）
        /// </summary>
        [Key]
        public long post_id { get; set; }
        
        /// <summary>
        /// 當時的遊戲ID
        /// </summary>
        [Required]
        public int game_id { get; set; }
        
        /// <summary>
        /// 拍照日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }
        
        /// <summary>
        /// 當日指數
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal index_value { get; set; }
        
        /// <summary>
        /// 當日各指標值/Δ%/權重（JSON）
        /// </summary>
        [StringLength(2000)]
        public string details_json { get; set; } = string.Empty;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：貼文
        /// </summary>
        public virtual Post Post { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
    }
}