using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 遊戲每日指標表
    /// </summary>
    [Table("game_metric_dailies")]
    public class GameMetricDaily
    {
        /// <summary>
        /// 記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long record_id { get; set; }

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id）
        /// </summary>
        [Required]
        public int game_id { get; set; }

        /// <summary>
        /// 指標ID（外鍵參考 metrics.metric_id）
        /// </summary>
        [Required]
        public int metric_id { get; set; }

        /// <summary>
        /// 記錄日期
        /// </summary>
        [Required]
        public DateTime record_date { get; set; }

        /// <summary>
        /// 指標值
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal metric_value { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 遊戲
        /// </summary>
        [ForeignKey("game_id")]
        public virtual Game Game { get; set; } = null!;

        /// <summary>
        /// 指標
        /// </summary>
        [ForeignKey("metric_id")]
        public virtual Metric Metric { get; set; } = null!;
    }
}