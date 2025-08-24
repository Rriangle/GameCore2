using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 指標字典實體 - 來源底下的可用指標清單
/// 對應資料表: metrics
/// </summary>
[Table("metrics")]
public class Metric
{
    /// <summary>
    /// 指標編號 (主鍵)
    /// </summary>
    [Key]
    [Column("metric_id")]
    public int MetricId { get; set; }

    /// <summary>
    /// 所屬來源編號 (外鍵至 MetricSource)
    /// </summary>
    [Required]
    [Column("source_id")]
    public int SourceId { get; set; }

    /// <summary>
    /// 指標代碼 (concurrent_users/forum_posts等)
    /// </summary>
    [Required]
    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 單位 (users/posts/views等)
    /// </summary>
    [Required]
    [Column("unit")]
    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 指標中文說明
    /// </summary>
    [Required]
    [Column("description")]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 指標權重 (用於計算熱度指數)
    /// </summary>
    [Column("weight", TypeName = "decimal(5,4)")]
    public decimal? Weight { get; set; }

    /// <summary>
    /// 指標類型 (primary/secondary/auxiliary)
    /// </summary>
    [Column("metric_type")]
    [StringLength(20)]
    public string? MetricType { get; set; }

    /// <summary>
    /// 計算方法說明
    /// </summary>
    [Column("calculation_note")]
    [StringLength(500)]
    public string? CalculationNote { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬數據來源
    /// </summary>
    [ForeignKey("SourceId")]
    public virtual MetricSource Source { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 此指標的每日數據
    /// </summary>
    public virtual ICollection<GameMetricDaily> DailyData { get; set; } = new List<GameMetricDaily>();
}