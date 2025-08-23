using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組成員表
    /// </summary>
    [Table("group_members")]
    public class GroupMember
    {
        /// <summary>
        /// 成員記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long member_record_id { get; set; }

        /// <summary>
        /// 群組ID（外鍵參考 groups.group_id）
        /// </summary>
        [Required]
        public int group_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 成員角色（member/moderator/admin）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string member_role { get; set; } = "member";

        /// <summary>
        /// 成員狀態（active/inactive/banned）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string member_status { get; set; } = "active";

        /// <summary>
        /// 加入時間
        /// </summary>
        public DateTime joined_at { get; set; } = DateTime.UtcNow;

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
        /// 群組
        /// </summary>
        [ForeignKey("group_id")]
        public virtual Group Group { get; set; } = null!;

        /// <summary>
        /// 用戶
        /// </summary>
        [ForeignKey("user_id")]
        public virtual User User { get; set; } = null!;
    }
}