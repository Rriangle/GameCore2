using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 自由市場排行榜表
    /// </summary>
    [Table("player_market_ranking")]
    public class PlayerMarketRanking
    {
        /// <summary>
        /// 排行榜ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ranking_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 player_market_product_info.product_id）
        /// </summary>
        [Required]
        public long product_id { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        [Required]
        public int rank { get; set; }

        /// <summary>
        /// 瀏覽次數
        /// </summary>
        public int view_count { get; set; } = 0;

        /// <summary>
        /// 收藏次數
        /// </summary>
        public int favorite_count { get; set; } = 0;

        /// <summary>
        /// 排行榜類型（daily/weekly/monthly）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ranking_type { get; set; } = "daily";

        /// <summary>
        /// 排行榜日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime ranking_date { get; set; }

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