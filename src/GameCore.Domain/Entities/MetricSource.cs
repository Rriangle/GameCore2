using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 數據來源字典：定義要抓的外部平台
    /// </summary>
    [Table("metric_sources")]
    public class MetricSource
    {
        /// <summary>
        /// 來源ID（主鍵）
        /// </summary>
        [Key]
        public int source_id { get; set; }
        
        /// <summary>
        /// 來源名（Steam/Bahamut/YouTube等）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string name { get; set; } = string.Empty;
        
        /// <summary>
        /// 備註（抓法/限制）
        /// </summary>
        [StringLength(500)]
        public string note { get; set; } = string.Empty;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：指標清單
        /// </summary>
        public virtual ICollection<Metric> Metrics { get; set; } = new List<Metric>();
        
        /// <summary>
        /// 導航屬性：遊戲來源對應
        /// </summary>
        public virtual ICollection<GameSourceMap> GameSourceMaps { get; set; } = new List<GameSourceMap>();
    }
}