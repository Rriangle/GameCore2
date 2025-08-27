using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 通知來源表
    /// </summary>
    [Table("notification_sources")]
    public class NotificationSource
    {
        /// <summary>
        /// 來源ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int source_id { get; set; }

        /// <summary>
        /// 來源名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string source_name { get; set; } = string.Empty;

        /// <summary>
        /// 來源描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 來源類型（system/user/order/forum等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string source_type { get; set; } = string.Empty;

        /// <summary>
        /// 來源狀態（active/inactive）
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