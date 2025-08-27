using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 用戶權限表
    /// </summary>
    [Table("user_rights")]
    public class UserRights
    {
        /// <summary>
        /// 權限ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int rights_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 權限類型（post/comment/market/admin等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string permission_type { get; set; } = string.Empty;

        /// <summary>
        /// 權限狀態（granted/revoked）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "granted";

        /// <summary>
        /// 權限描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 權限授予時間
        /// </summary>
        public DateTime granted_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 權限撤銷時間
        /// </summary>
        public DateTime? revoked_at { get; set; }

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