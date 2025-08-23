using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 用戶簽到統計表
    /// </summary>
    [Table("user_sign_in_stats")]
    public class UserSignInStats
    {
        /// <summary>
        /// 記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long record_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 簽到日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime sign_in_date { get; set; }

        /// <summary>
        /// 連續簽到天數
        /// </summary>
        public int consecutive_days { get; set; } = 1;

        /// <summary>
        /// 簽到時間
        /// </summary>
        public DateTime sign_in_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 獲得積分
        /// </summary>
        public int points_earned { get; set; } = 0;

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