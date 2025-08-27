using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 小遊戲表
    /// </summary>
    [Table("mini_games")]
    public class MiniGame
    {
        /// <summary>
        /// 遊戲記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long game_record_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 遊戲類型（memory/puzzle/quiz等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string game_type { get; set; } = string.Empty;

        /// <summary>
        /// 遊戲分數
        /// </summary>
        public int score { get; set; } = 0;

        /// <summary>
        /// 遊戲時間（秒）
        /// </summary>
        public int game_duration { get; set; } = 0;

        /// <summary>
        /// 遊戲結果（win/lose/draw）
        /// </summary>
        [StringLength(20)]
        public string? result { get; set; }

        /// <summary>
        /// 獲得積分
        /// </summary>
        public int points_earned { get; set; } = 0;

        /// <summary>
        /// 遊戲開始時間
        /// </summary>
        public DateTime game_start_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 遊戲結束時間
        /// </summary>
        public DateTime? game_end_time { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 用戶
        /// </summary>
        [ForeignKey("user_id")]
        public virtual User User { get; set; } = null!;
    }
}