using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 供應商表
    /// </summary>
    [Table("suppliers")]
    public class Supplier
    {
        /// <summary>
        /// 供應商ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int supplier_id { get; set; }

        /// <summary>
        /// 供應商名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string supplier_name { get; set; } = string.Empty;

        /// <summary>
        /// 供應商描述
        /// </summary>
        [StringLength(1000)]
        public string? description { get; set; }

        /// <summary>
        /// 聯絡電話
        /// </summary>
        [StringLength(20)]
        public string? phone { get; set; }

        /// <summary>
        /// 電子郵件
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string? email { get; set; }

        /// <summary>
        /// 聯絡地址
        /// </summary>
        [StringLength(500)]
        public string? address { get; set; }

        /// <summary>
        /// 供應商狀態（active/inactive）
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