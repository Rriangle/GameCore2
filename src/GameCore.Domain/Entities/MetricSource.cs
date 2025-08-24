using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 數據來源字典實體 - 定義要抓取的外部平台
/// 對應資料表: metric_sources
/// </summary>
[Table("metric_sources")]
public class MetricSource
{
    /// <summary>
    /// 來源編號 (主鍵)
    /// </summary>
    [Key]
    [Column("source_id")]
    public int SourceId { get; set; }

    /// <summary>
    /// 來源名稱 (Steam/Bahamut/YouTube等)
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 備註 (抓法/限制等說明)
    /// </summary>
    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 來源類型 (api/scraping/manual)
    /// </summary>
    [Column("source_type")]
    [StringLength(20)]
    public string? SourceType { get; set; }

    /// <summary>
    /// API基礎URL
    /// </summary>
    [Column("base_url")]
    [StringLength(500)]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 更新頻率 (小時)
    /// </summary>
    [Column("update_frequency_hours")]
    public int? UpdateFrequencyHours { get; set; }

    /// <summary>
    /// 導航屬性 - 此來源的指標定義
    /// </summary>
    public virtual ICollection<Metric> Metrics { get; set; } = new List<Metric>();

    /// <summary>
    /// 導航屬性 - 此來源的遊戲對應
    /// </summary>
    public virtual ICollection<GameSourceMap> GameMaps { get; set; } = new List<GameSourceMap>();
}