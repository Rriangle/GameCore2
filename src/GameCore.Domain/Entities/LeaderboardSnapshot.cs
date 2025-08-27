using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 榜單快照：直接給前台讀，避免重算
    /// </summary>
    [Table("leaderboard_snapshots")]
    public class LeaderboardSnapshot
    {
        /// <summary>
        /// 快照ID
        /// </summary>
        [Key]
        public long snapshot_id { get; set; }
        
        /// <summary>
        /// 期間類型（daily/weekly）
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
        /// 遊戲ID
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
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
    }
}