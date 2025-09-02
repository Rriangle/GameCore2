using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 官方商城排行榜表
/// 對應資料庫 Official_Store_Ranking 表
/// </summary>
[Table("Official_Store_Ranking")]
public class OfficialStoreRanking
{
    /// <summary>
    /// 排行榜流水號 (主鍵)
    /// </summary>
    [Key]
    public int RankingID { get; set; }

    /// <summary>
    /// 榜單型態 (日、月、季、年)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string PeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 榜單日期 (計算日期)
    /// </summary>
    public DateTime RankingDate { get; set; }

    /// <summary>
    /// 指向排名目標商品ID
    /// </summary>
    public int ProductID { get; set; }

    /// <summary>
    /// 排名指標 (交易額/量)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 名次
    /// </summary>
    public byte RankingPosition { get; set; }

    /// <summary>
    /// 交易額
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TradingAmount { get; set; }

    /// <summary>
    /// 交易量
    /// </summary>
    public int TradingVolume { get; set; }

    /// <summary>
    /// 排行榜更新時間
    /// </summary>
    public DateTime RankingUpdatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的商品
    /// </summary>
    public virtual ProductInfo ProductInfo { get; set; } = null!;
} 