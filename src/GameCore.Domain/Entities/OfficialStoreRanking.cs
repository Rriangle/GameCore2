using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 官方商城排行榜表
    /// </summary>
    [Table("official_store_ranking")]
    public class OfficialStoreRanking
    {
        /// <summary>
        /// 排行榜ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ranking_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 product_info.product_id）
        /// </summary>
        [Required]
        public int product_id { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        [Required]
        public int rank { get; set; }

        /// <summary>
        /// 銷售量
        /// </summary>
        public int sales_volume { get; set; } = 0;

        /// <summary>
        /// 銷售額
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal sales_amount { get; set; } = 0.00m;

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
        public virtual ProductInfo Product { get; set; } = null!;
    }
}