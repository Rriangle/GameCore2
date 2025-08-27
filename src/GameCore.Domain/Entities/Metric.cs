using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 指標字典：來源底下的可用指標清單
    /// </summary>
    [Table("metrics")]
    public class Metric
    {
        /// <summary>
        /// 指標ID（主鍵）
        /// </summary>
        [Key]
        public int metric_id { get; set; }
        
        /// <summary>
        /// 所屬來源
        /// </summary>
        [Required]
        public int source_id { get; set; }
        
        /// <summary>
        /// 指標代碼（concurrent_users/forum_posts等）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string code { get; set; } = string.Empty;
        
        /// <summary>
        /// 單位（users/posts/views）
        /// </summary>
        [StringLength(50)]
        public string unit { get; set; } = string.Empty;
        
        /// <summary>
        /// 指標中文說明
        /// </summary>
        [StringLength(200)]
        public string description { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否啟用
        /// </summary>
        [Required]
        public bool is_active { get; set; } = true;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：所屬來源
        /// </summary>
        public virtual MetricSource Source { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：每日指標數據
        /// </summary>
        public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
    }
}