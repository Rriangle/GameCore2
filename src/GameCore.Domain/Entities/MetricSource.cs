using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 指標來源表
/// 對應資料庫 metric_sources 表
/// </summary>
[Table("metric_sources")]
public class MetricSource
{
    /// <summary>
    /// 來源ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SourceID { get; set; }

    /// <summary>
    /// 來源名稱 (Steam/Bahamut/YouTube...)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 備註 (抓法/限制)
    /// </summary>
    [StringLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 來源下的指標
    /// </summary>
    public virtual ICollection<Metric> Metrics { get; set; } = new List<Metric>();

    /// <summary>
    /// 遊戲來源對應
    /// </summary>
    public virtual ICollection<GameSourceMap> GameSourceMaps { get; set; } = new List<GameSourceMap>();
} 