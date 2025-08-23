using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 訂單明細表
    /// </summary>
    [Table("order_items")]
    public class OrderItem
    {
        /// <summary>
        /// 明細ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long item_id { get; set; }

        /// <summary>
        /// 訂單ID（外鍵參考 order_info.order_id）
        /// </summary>
        [Required]
        public long order_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 product_info.product_id）
        /// </summary>
        [Required]
        public int product_id { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        [Required]
        public int quantity { get; set; } = 1;

        /// <summary>
        /// 單價
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal unit_price { get; set; }

        /// <summary>
        /// 小計
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal subtotal { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 訂單
        /// </summary>
        [ForeignKey("order_id")]
        public virtual OrderInfo Order { get; set; } = null!;

        /// <summary>
        /// 商品
        /// </summary>
        [ForeignKey("product_id")]
        public virtual ProductInfo Product { get; set; } = null!;
    }
}