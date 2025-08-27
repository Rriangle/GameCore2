using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 每天每指標的原始值（清洗後），是計算指數的底稿
    /// </summary>
    [Table("game_metric_daily")]
    public class GameMetricDaily
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        public int id { get; set; }
        
        /// <summary>
        /// 遊戲ID
        /// </summary>
        [Required]
        public int game_id { get; set; }
        
        /// <summary>
        /// 指標ID
        /// </summary>
        [Required]
        public int metric_id { get; set; }
        
        /// <summary>
        /// 日期（日粒度）
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }
        
        /// <summary>
        /// 數值（清洗後）
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal value { get; set; }
        
        /// <summary>
        /// 聚合方法（sum/avg/max）
        /// </summary>
        [StringLength(20)]
        public string agg_method { get; set; } = string.Empty;
        
        /// <summary>
        /// 資料品質（real/estimate/seed）
        /// </summary>
        [StringLength(20)]
        public string quality { get; set; } = string.Empty;
        
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
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：指標
        /// </summary>
        public virtual Metric Metric { get; set; } = null!;
    }
}