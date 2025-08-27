using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 官方商城排行榜實體 - 商品銷售排行統計
/// 對應資料表: Official_Store_Ranking (OSR)
/// </summary>
[Table("Official_Store_Ranking")]
public class OfficialStoreRanking
{
    /// <summary>
    /// 排行榜流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("ranking_id")]
    public int RankingId { get; set; }

    /// <summary>
    /// 榜單型態 (日、月、季、年)
    /// </summary>
    [Required]
    [Column("period_type")]
    [StringLength(20)]
    public string PeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 榜單日期 (計算日期)
    /// </summary>
    [Required]
    [Column("ranking_date")]
    public DateOnly RankingDate { get; set; }

    /// <summary>
    /// 商品編號 (外鍵至 ProductInfo)
    /// </summary>
    [Required]
    [Column("product_ID")]
    public int ProductId { get; set; }

    /// <summary>
    /// 排名指標 (交易額/量)
    /// </summary>
    [Required]
    [Column("ranking_metric")]
    [StringLength(50)]
    public string RankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 名次
    /// </summary>
    [Required]
    [Column("ranking_position")]
    public byte RankingPosition { get; set; }

    /// <summary>
    /// 交易額
    /// </summary>
    [Required]
    [Column("trading_amount", TypeName = "decimal(18,2)")]
    public decimal TradingAmount { get; set; }

    /// <summary>
    /// 交易量
    /// </summary>
    [Required]
    [Column("trading_volume")]
    public int TradingVolume { get; set; }

    /// <summary>
    /// 排行榜更新時間
    /// </summary>
    [Required]
    [Column("ranking_updated_at")]
    public DateTime RankingUpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 排名的商品
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual ProductInfo Product { get; set; } = null!;
}