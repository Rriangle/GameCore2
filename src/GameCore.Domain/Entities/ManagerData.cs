using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理員資料表
    /// </summary>
    [Table("manager_data")]
    public class ManagerData
    {
        /// <summary>
        /// 管理員ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int manager_id { get; set; }

        /// <summary>
        /// 管理員姓名
        /// </summary>
        [Required]
        [StringLength(100)]
        public string manager_name { get; set; } = string.Empty;

        /// <summary>
        /// 管理員帳號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string manager_account { get; set; } = string.Empty;

        /// <summary>
        /// 管理員密碼雜湊
        /// </summary>
        [Required]
        [StringLength(255)]
        public string password_hash { get; set; } = string.Empty;

        /// <summary>
        /// 管理員角色（admin/moderator/support）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string role { get; set; } = "support";

        /// <summary>
        /// 管理員狀態（active/inactive/suspended）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime? last_login_at { get; set; }

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