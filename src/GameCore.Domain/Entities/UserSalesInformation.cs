using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 用戶銷售資訊表
    /// </summary>
    [Table("user_sales_information")]
    public class UserSalesInformation
    {
        /// <summary>
        /// 銷售資訊ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sales_info_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 銀行代號
        /// </summary>
        [Required]
        public int bank_code { get; set; }

        /// <summary>
        /// 銀行帳號
        /// </summary>
        [Required]
        [StringLength(50)]
        public string bank_account_number { get; set; } = string.Empty;

        /// <summary>
        /// 銀行名稱
        /// </summary>
        [StringLength(100)]
        public string? bank_name { get; set; }

        /// <summary>
        /// 帳戶持有人姓名
        /// </summary>
        [Required]
        [StringLength(100)]
        public string account_holder_name { get; set; } = string.Empty;

        /// <summary>
        /// 申請狀態（pending/approved/rejected）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string application_status { get; set; } = "pending";

        /// <summary>
        /// 申請時間
        /// </summary>
        public DateTime application_date { get; set; } = DateTime.UtcNow;

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
        /// 用戶
        /// </summary>
        [ForeignKey("user_id")]
        public virtual User User { get; set; } = null!;
    }
}