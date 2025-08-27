using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理員角色權限表
    /// </summary>
    [Table("manager_role_permission")]
    public class ManagerRolePermission
    {
        /// <summary>
        /// 權限ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int permission_id { get; set; }

        /// <summary>
        /// 權限名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string permission_name { get; set; } = string.Empty;

        /// <summary>
        /// 權限描述
        /// </summary>
        [StringLength(500)]
        public string? permission_description { get; set; }

        /// <summary>
        /// 權限類型（read/write/admin等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string permission_type { get; set; } = string.Empty;

        /// <summary>
        /// 權限範圍（user/order/product/forum等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string permission_scope { get; set; } = string.Empty;

        /// <summary>
        /// 權限狀態（active/inactive）
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