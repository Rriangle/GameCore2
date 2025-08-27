using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 樣式表
    /// </summary>
    [Table("styles")]
    public class Style
    {
        /// <summary>
        /// 樣式ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int style_id { get; set; }

        /// <summary>
        /// 管理員ID（外鍵參考 manager_data.manager_id）
        /// </summary>
        [Required]
        public int manager_id { get; set; }

        /// <summary>
        /// 樣式名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string style_name { get; set; } = string.Empty;

        /// <summary>
        /// 樣式描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 樣式類型（theme/layout/color等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string style_type { get; set; } = string.Empty;

        /// <summary>
        /// 樣式內容（CSS/JSON等）
        /// </summary>
        [Required]
        public string style_content { get; set; } = string.Empty;

        /// <summary>
        /// 樣式狀態（active/inactive/draft）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "draft";

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
        /// 管理員
        /// </summary>
        [ForeignKey("manager_id")]
        public virtual ManagerData Manager { get; set; } = null!;
    }
}