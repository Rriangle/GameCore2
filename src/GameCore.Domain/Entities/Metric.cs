using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 指標表
    /// </summary>
    [Table("metrics")]
    public class Metric
    {
        /// <summary>
        /// 指標ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int metric_id { get; set; }

        /// <summary>
        /// 指標名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string metric_name { get; set; } = string.Empty;

        /// <summary>
        /// 指標描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 指標類型（count/percentage/currency等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string metric_type { get; set; } = string.Empty;

        /// <summary>
        /// 指標單位
        /// </summary>
        [StringLength(20)]
        public string? unit { get; set; }

        /// <summary>
        /// 指標狀態（active/inactive）
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