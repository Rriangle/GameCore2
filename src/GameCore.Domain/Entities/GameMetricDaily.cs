using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 每天每指標的原始值表
/// 對應資料庫 game_metric_daily 表
/// </summary>
[Table("game_metric_daily")]
public class GameMetricDaily
{
    /// <summary>
    /// 流水號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    /// <summary>
    /// 遊戲ID
    /// </summary>
    public int GameID { get; set; }

    /// <summary>
    /// 指標ID
    /// </summary>
    public int MetricID { get; set; }

    /// <summary>
    /// 日期 (日粒度)
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 數值 (清洗後)
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal Value { get; set; }

    /// <summary>
    /// 聚合方法 (sum/avg/max)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AggMethod { get; set; } = string.Empty;

    /// <summary>
    /// 資料品質 (real/estimate/seed)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Quality { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的遊戲
    /// </summary>
    public virtual Game Game { get; set; } = null!;

    /// <summary>
    /// 關聯的指標
    /// </summary>
    public virtual Metric Metric { get; set; } = null!;
} 