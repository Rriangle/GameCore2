using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理員登入追蹤表
    /// </summary>
    [Table("admin")]
    public class Admin
    {
        /// <summary>
        /// 登入記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long login_record_id { get; set; }

        /// <summary>
        /// 管理員ID（外鍵參考 manager_data.manager_id）
        /// </summary>
        [Required]
        public int manager_id { get; set; }

        /// <summary>
        /// 登入時間
        /// </summary>
        public DateTime login_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 登出時間
        /// </summary>
        public DateTime? logout_time { get; set; }

        /// <summary>
        /// 登入IP地址
        /// </summary>
        [StringLength(45)]
        public string? login_ip { get; set; }

        /// <summary>
        /// 登入裝置
        /// </summary>
        [StringLength(200)]
        public string? login_device { get; set; }

        /// <summary>
        /// 登入瀏覽器
        /// </summary>
        [StringLength(100)]
        public string? login_browser { get; set; }

        /// <summary>
        /// 登入狀態（success/failed）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string login_status { get; set; } = "success";

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 管理員
        /// </summary>
        [ForeignKey("manager_id")]
        public virtual ManagerData Manager { get; set; } = null!;
    }
}