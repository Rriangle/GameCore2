using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 自由市場商品表
    /// </summary>
    [Table("player_market_product_info")]
    public class PlayerMarketProductInfo
    {
        /// <summary>
        /// 商品ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long product_id { get; set; }

        /// <summary>
        /// 賣家ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int seller_id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string product_name { get; set; } = string.Empty;

        /// <summary>
        /// 商品描述
        /// </summary>
        [StringLength(1000)]
        public string? description { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal price { get; set; }

        /// <summary>
        /// 商品數量
        /// </summary>
        [Required]
        public int quantity { get; set; } = 1;

        /// <summary>
        /// 商品狀態（active/sold/expired/cancelled）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 商品建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 商品更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        /// <summary>
        /// 商品下架時間
        /// </summary>
        public DateTime? expired_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 賣家
        /// </summary>
        [ForeignKey("seller_id")]
        public virtual User Seller { get; set; } = null!;
    }
}