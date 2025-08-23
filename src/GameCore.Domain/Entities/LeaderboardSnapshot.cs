using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 榜單快照表
    /// </summary>
    [Table("leaderboard_snapshots")]
    public class LeaderboardSnapshot
    {
        /// <summary>
        /// 快照ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long snapshot_id { get; set; }

        /// <summary>
        /// 期間類型（daily/weekly/monthly）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string period { get; set; } = string.Empty;

        /// <summary>
        /// 快照時間
        /// </summary>
        [Required]
        public DateTime ts { get; set; }

        /// <summary>
        /// 名次（1..N）
        /// </summary>
        [Required]
        public int rank { get; set; }

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id）
        /// </summary>
        [Required]
        public int game_id { get; set; }

        /// <summary>
        /// 當時指數值
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal index_value { get; set; }

        /// <summary>
        /// 指數變化率
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? change_rate { get; set; }

        /// <summary>
        /// 指數變化量
        /// </summary>
        [Column(TypeName = "decimal(18,4)")]
        public decimal? change_amount { get; set; }

        /// <summary>
        /// 指數等級
        /// </summary>
        [StringLength(10)]
        public string? index_grade { get; set; }

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
    }
}