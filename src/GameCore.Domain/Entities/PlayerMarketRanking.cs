using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場排行榜實體 - C2C商品的排行榜數據
/// 對應資料表: PlayerMarketRanking (PMR)
/// </summary>
[Table("PlayerMarketRanking")]
public class PlayerMarketRanking
{
    /// <summary>
    /// 自由市場排行榜編號 (主鍵)
    /// </summary>
    [Key]
    [Column("p_ranking_id")]
    public int PRankingId { get; set; }

    /// <summary>
    /// 自由市場榜單型態 (daily/weekly/monthly)
    /// </summary>
    [Required]
    [Column("p_period_type")]
    [StringLength(20)]
    public string PPeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 自由市場排行榜日期
    /// </summary>
    [Required]
    [Column("p_ranking_date")]
    public DateOnly PRankingDate { get; set; }

    /// <summary>
    /// 自由市場商品編號 (外鍵至 PlayerMarketProductInfo)
    /// </summary>
    [Required]
    [Column("p_product_id")]
    public int PProductId { get; set; }

    /// <summary>
    /// 排名指標 (amount/volume/views)
    /// </summary>
    [Required]
    [Column("p_ranking_metric")]
    [StringLength(50)]
    public string PRankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 自由市場排名位置
    /// </summary>
    [Required]
    [Column("p_ranking_position")]
    public byte PRankingPosition { get; set; }

    /// <summary>
    /// 自由市場交易額
    /// </summary>
    [Required]
    [Column("p_trading_amount", TypeName = "decimal(18,2)")]
    public decimal PTradingAmount { get; set; }

    /// <summary>
    /// 自由市場交易量
    /// </summary>
    [Required]
    [Column("p_trading_volume")]
    public int PTradingVolume { get; set; }

    /// <summary>
    /// 排行榜更新時間
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 商品瀏覽次數 (用於瀏覽量排名)
    /// </summary>
    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 商品收藏次數 (用於人氣排名)
    /// </summary>
    [Column("favorite_count")]
    public int FavoriteCount { get; set; } = 0;

    /// <summary>
    /// 賣家評分平均 (用於信譽排名)
    /// </summary>
    [Column("seller_rating", TypeName = "decimal(3,2)")]
    public decimal? SellerRating { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 排名的商品
    /// </summary>
    [ForeignKey("PProductId")]
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;
}