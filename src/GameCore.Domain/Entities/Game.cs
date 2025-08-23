using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 遊戲表
    /// </summary>
    [Table("games")]
    public class Game
    {
        /// <summary>
        /// 遊戲ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int game_id { get; set; }

        /// <summary>
        /// 遊戲名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 遊戲類型
        /// </summary>
        [StringLength(50)]
        public string? category { get; set; }

        /// <summary>
        /// 遊戲狀態（active/inactive/maintenance）
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
        /// 排行榜快照
        /// </summary>
        public virtual ICollection<LeaderboardSnapshot> LeaderboardSnapshots { get; set; } = new List<LeaderboardSnapshot>();
    }
}