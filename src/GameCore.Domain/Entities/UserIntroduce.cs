using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 用戶介紹表
    /// </summary>
    [Table("user_introduce")]
    public class UserIntroduce
    {
        /// <summary>
        /// 介紹ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int introduce_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 用戶暱稱
        /// </summary>
        [StringLength(100)]
        public string? nickname { get; set; }

        /// <summary>
        /// 用戶頭像URL
        /// </summary>
        [StringLength(500)]
        public string? avatar_url { get; set; }

        /// <summary>
        /// 用戶簡介
        /// </summary>
        [StringLength(1000)]
        public string? bio { get; set; }

        /// <summary>
        /// 用戶生日
        /// </summary>
        public DateTime? birth_date { get; set; }

        /// <summary>
        /// 用戶性別（male/female/other）
        /// </summary>
        [StringLength(20)]
        public string? gender { get; set; }

        /// <summary>
        /// 用戶所在地
        /// </summary>
        [StringLength(200)]
        public string? location { get; set; }

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
        /// 用戶
        /// </summary>
        [ForeignKey("user_id")]
        public virtual User User { get; set; } = null!;
    }
}