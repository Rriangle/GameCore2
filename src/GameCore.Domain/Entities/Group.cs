using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組表
    /// </summary>
    [Table("groups")]
    public class Group
    {
        /// <summary>
        /// 群組ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int group_id { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string group_name { get; set; } = string.Empty;

        /// <summary>
        /// 群組描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 群組類型（public/private/secret）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string group_type { get; set; } = "public";

        /// <summary>
        /// 群組狀態（active/inactive/archived）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 群組建立者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int creator_id { get; set; }

        /// <summary>
        /// 群組建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 群組更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 群組建立者
        /// </summary>
        [ForeignKey("creator_id")]
        public virtual User Creator { get; set; } = null!;

        /// <summary>
        /// 群組成員
        /// </summary>
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

        /// <summary>
        /// 群組聊天訊息
        /// </summary>
        public virtual ICollection<GroupChat> GroupChatMessages { get; set; } = new List<GroupChat>();
    }
}