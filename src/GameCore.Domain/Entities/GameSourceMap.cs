using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 遊戲指標來源對應表
    /// </summary>
    [Table("game_source_maps")]
    public class GameSourceMap
    {
        /// <summary>
        /// 對應ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int map_id { get; set; }

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id）
        /// </summary>
        [Required]
        public int game_id { get; set; }

        /// <summary>
        /// 指標來源ID（外鍵參考 metric_sources.source_id）
        /// </summary>
        [Required]
        public int source_id { get; set; }

        /// <summary>
        /// 對應狀態（active/inactive）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 遊戲
        /// </summary>
        [ForeignKey("game_id")]
        public virtual Game Game { get; set; } = null!;

        /// <summary>
        /// 指標來源
        /// </summary>
        [ForeignKey("source_id")]
        public virtual MetricSource MetricSource { get; set; } = null!;
    }
}