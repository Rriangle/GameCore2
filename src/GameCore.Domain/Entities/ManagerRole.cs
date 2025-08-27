using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理員角色表
    /// </summary>
    [Table("manager_role")]
    public class ManagerRole
    {
        /// <summary>
        /// 角色ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int role_id { get; set; }

        /// <summary>
        /// 管理員ID（外鍵參考 manager_data.manager_id）
        /// </summary>
        [Required]
        public int manager_id { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string role_name { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string? role_description { get; set; }

        /// <summary>
        /// 角色權限（JSON格式）
        /// </summary>
        public string? permissions { get; set; }

        /// <summary>
        /// 角色狀態（active/inactive）
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

        // 導航屬性
        /// <summary>
        /// 管理員
        /// </summary>
        [ForeignKey("manager_id")]
        public virtual ManagerData Manager { get; set; } = null!;
    }
}