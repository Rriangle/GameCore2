using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 官方商城訂單表
    /// </summary>
    [Table("order_info")]
    public class OrderInfo
    {
        /// <summary>
        /// 訂單ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long order_id { get; set; }

        /// <summary>
        /// 買家ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int buyer_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 game_product_details.detail_id）
        /// </summary>
        [Required]
        public int product_id { get; set; }

        /// <summary>
        /// 訂單狀態（pending/paid/shipped/delivered/cancelled）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "pending";

        /// <summary>
        /// 訂單金額
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal amount { get; set; }

        /// <summary>
        /// 訂單建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 付款時間
        /// </summary>
        public DateTime? paid_at { get; set; }

        /// <summary>
        /// 出貨時間
        /// </summary>
        public DateTime? shipped_at { get; set; }

        /// <summary>
        /// 送達時間
        /// </summary>
        public DateTime? delivered_at { get; set; }

        /// <summary>
        /// 取消時間
        /// </summary>
        public DateTime? cancelled_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 買家
        /// </summary>
        [ForeignKey("buyer_id")]
        public virtual User Buyer { get; set; } = null!;

        /// <summary>
        /// 商品
        /// </summary>
        [ForeignKey("product_id")]
        public virtual GameProductDetails Product { get; set; } = null!;
    }
}