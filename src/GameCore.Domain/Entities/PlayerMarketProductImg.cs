using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 自由市場商品圖片表
    /// </summary>
    [Table("player_market_product_img")]
    public class PlayerMarketProductImg
    {
        /// <summary>
        /// 圖片ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long img_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 player_market_product_info.product_id）
        /// </summary>
        [Required]
        public long product_id { get; set; }

        /// <summary>
        /// 圖片URL
        /// </summary>
        [Required]
        [StringLength(500)]
        public string image_url { get; set; } = string.Empty;

        /// <summary>
        /// 圖片類型（main/thumbnail/additional）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string image_type { get; set; } = "additional";

        /// <summary>
        /// 圖片順序
        /// </summary>
        public int sort_order { get; set; } = 0;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 商品
        /// </summary>
        [ForeignKey("product_id")]
        public virtual PlayerMarketProductInfo Product { get; set; } = null!;
    }
}