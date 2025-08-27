using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知行為表
    /// </summary>
    [Table("notification_actions")]
    public class NotificationAction
    {
        /// <summary>
        /// 行為ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int action_id { get; set; }

        /// <summary>
        /// 行為名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string action_name { get; set; } = string.Empty;

        /// <summary>
        /// 行為描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 行為類型（create/update/delete/status_change等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string action_type { get; set; } = string.Empty;

        /// <summary>
        /// 行為狀態（active/inactive）
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
    }
}