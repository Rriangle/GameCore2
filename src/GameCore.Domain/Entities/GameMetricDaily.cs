using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 遊戲每日指標數據實體 - 每天每指標的原始值（清洗後），是計算指數的底稿
/// 對應資料表: game_metric_daily
/// </summary>
[Table("game_metric_daily")]
public class GameMetricDaily
{
    /// <summary>
    /// 流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 遊戲編號 (外鍵至 Game)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 指標編號 (外鍵至 Metric)
    /// </summary>
    [Required]
    [Column("metric_id")]
    public int MetricId { get; set; }

    /// <summary>
    /// 日期 (日粒度)
    /// </summary>
    [Required]
    [Column("date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// 數值 (清洗後)
    /// </summary>
    [Required]
    [Column("value", TypeName = "decimal(18,4)")]
    public decimal Value { get; set; }

    /// <summary>
    /// 聚合方法 (sum/avg/max等)
    /// </summary>
    [Required]
    [Column("agg_method")]
    [StringLength(20)]
    public string AggMethod { get; set; } = string.Empty;

    /// <summary>
    /// 資料品質 (real/estimate/seed)
    /// </summary>
    [Required]
    [Column("quality")]
    [StringLength(20)]
    public string Quality { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 原始值 (未清洗前)
    /// </summary>
    [Column("raw_value", TypeName = "decimal(18,4)")]
    public decimal? RawValue { get; set; }

    /// <summary>
    /// 變化百分比 (與前一日比較)
    /// </summary>
    [Column("change_percentage", TypeName = "decimal(8,4)")]
    public decimal? ChangePercentage { get; set; }

    /// <summary>
    /// 數據來源說明
    /// </summary>
    [Column("source_note")]
    [StringLength(200)]
    public string? SourceNote { get; set; }

    /// <summary>
    /// 是否為異常值
    /// </summary>
    [Column("is_outlier")]
    public bool IsOutlier { get; set; } = false;

    /// <summary>
    /// 導航屬性 - 所屬遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 所屬指標
    /// </summary>
    [ForeignKey("MetricId")]
    public virtual Metric Metric { get; set; } = null!;
}