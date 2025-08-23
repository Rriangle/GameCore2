using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 會員銷售資料表
    /// </summary>
    [Table("member_sales_profile")]
    public class MemberSalesProfile
    {
        /// <summary>
        /// 銷售資料ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sales_profile_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 總銷售額
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal total_sales { get; set; } = 0.00m;

        /// <summary>
        /// 總訂單數
        /// </summary>
        public int total_orders { get; set; } = 0;

        /// <summary>
        /// 成功交易數
        /// </summary>
        public int successful_transactions { get; set; } = 0;

        /// <summary>
        /// 取消交易數
        /// </summary>
        public int cancelled_transactions { get; set; } = 0;

        /// <summary>
        /// 平均評分
        /// </summary>
        [Column(TypeName = "decimal(3,2)")]
        public decimal? average_rating { get; set; }

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