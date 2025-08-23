using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 商品資訊審計日誌表
    /// </summary>
    [Table("product_info_audit_log")]
    public class ProductInfoAuditLog
    {
        /// <summary>
        /// 日誌ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long log_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 product_info.product_id）
        /// </summary>
        [Required]
        public int product_id { get; set; }

        /// <summary>
        /// 管理員ID（外鍵參考 manager_data.manager_id）
        /// </summary>
        [Required]
        public int manager_id { get; set; }

        /// <summary>
        /// 操作類型（create/update/delete）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string action_type { get; set; } = string.Empty;

        /// <summary>
        /// 操作描述
        /// </summary>
        [StringLength(500)]
        public string? action_description { get; set; }

        /// <summary>
        /// 舊值（JSON格式）
        /// </summary>
        public string? old_values { get; set; }

        /// <summary>
        /// 新值（JSON格式）
        /// </summary>
        public string? new_values { get; set; }

        /// <summary>
        /// 操作時間
        /// </summary>
        public DateTime action_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 商品
        /// </summary>
        [ForeignKey("product_id")]
        public virtual ProductInfo Product { get; set; } = null!;

        /// <summary>
        /// 管理員
        /// </summary>
        [ForeignKey("manager_id")]
        public virtual ManagerData Manager { get; set; } = null!;
    }
}