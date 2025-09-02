using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場排行榜表
/// 對應資料庫 PlayerMarketRanking 表
/// </summary>
[Table("PlayerMarketRanking")]
public class PlayerMarketRanking
{
    /// <summary>
    /// 自由市場排行榜ID (主鍵)
    /// </summary>
    [Key]
    public int PRankingID { get; set; }

    /// <summary>
    /// 榜單型態
    /// </summary>
    [Required]
    [StringLength(20)]
    public string PPeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 榜單日期
    /// </summary>
    public DateTime PRankingDate { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public int PProductID { get; set; }

    /// <summary>
    /// 排名指標
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PRankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 名次
    /// </summary>
    public int PRankingPosition { get; set; }

    /// <summary>
    /// 交易額
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PTradingAmount { get; set; }

    /// <summary>
    /// 交易量
    /// </summary>
    public int PTradingVolume { get; set; }

    /// <summary>
    /// 排行榜更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的商品
    /// </summary>
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;
} 