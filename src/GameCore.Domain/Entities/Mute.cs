using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 禁言表
    /// </summary>
    [Table("mutes")]
    public class Mute
    {
        /// <summary>
        /// 禁言記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long mute_record_id { get; set; }

        /// <summary>
        /// 被禁言用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int muted_user_id { get; set; }

        /// <summary>
        /// 管理員ID（外鍵參考 manager_data.manager_id）
        /// </summary>
        [Required]
        public int manager_id { get; set; }

        /// <summary>
        /// 禁言原因
        /// </summary>
        [StringLength(500)]
        public string? mute_reason { get; set; }

        /// <summary>
        /// 禁言狀態（active/expired/removed）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string mute_status { get; set; } = "active";

        /// <summary>
        /// 禁言開始時間
        /// </summary>
        public DateTime mute_start_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 禁言結束時間
        /// </summary>
        public DateTime? mute_end_time { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 被禁言用戶
        /// </summary>
        [ForeignKey("muted_user_id")]
        public virtual User MutedUser { get; set; } = null!;

        /// <summary>
        /// 管理員
        /// </summary>
        [ForeignKey("manager_id")]
        public virtual ManagerData Manager { get; set; } = null!;
    }
}