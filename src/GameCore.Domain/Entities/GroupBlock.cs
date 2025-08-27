using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組封鎖表
    /// </summary>
    [Table("group_block")]
    public class GroupBlock
    {
        /// <summary>
        /// 封鎖記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long block_record_id { get; set; }

        /// <summary>
        /// 群組ID（外鍵參考 groups.group_id）
        /// </summary>
        [Required]
        public int group_id { get; set; }

        /// <summary>
        /// 被封鎖用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int blocked_user_id { get; set; }

        /// <summary>
        /// 封鎖原因
        /// </summary>
        [StringLength(500)]
        public string? block_reason { get; set; }

        /// <summary>
        /// 封鎖狀態（active/expired/removed）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string block_status { get; set; } = "active";

        /// <summary>
        /// 封鎖開始時間
        /// </summary>
        public DateTime block_start_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 封鎖結束時間
        /// </summary>
        public DateTime? block_end_time { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 群組
        /// </summary>
        [ForeignKey("group_id")]
        public virtual Group Group { get; set; } = null!;

        /// <summary>
        /// 被封鎖用戶
        /// </summary>
        [ForeignKey("blocked_user_id")]
        public virtual User BlockedUser { get; set; } = null!;
    }
}