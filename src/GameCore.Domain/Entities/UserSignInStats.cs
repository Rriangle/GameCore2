using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 用戶簽到統計實體
    /// </summary>
    [Table("UserSignInStats")]
    public class UserSignInStats
    {
        /// <summary>
        /// 簽到記錄 ID，自動遞增
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogID { get; set; }

        /// <summary>
        /// 簽到時間（預設 UTC 當下時間）
        /// </summary>
        [Required]
        public DateTime SignTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 會員 ID，外鍵參考 Users.UserID
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// 此次簽到會員點數增減數量
        /// </summary>
        [Required]
        public int PointsChanged { get; set; } = 0;

        /// <summary>
        /// 此次簽到寵物獲得經驗值
        /// </summary>
        [Required]
        public int ExpGained { get; set; } = 0;

        /// <summary>
        /// 點數變動時間
        /// </summary>
        [Required]
        public DateTime PointsChangedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 寵物經驗值獲得時間
        /// </summary>
        [Required]
        public DateTime ExpGainedTime { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 關聯的用戶
        /// </summary>
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
    }
} 