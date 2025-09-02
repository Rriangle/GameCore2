using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 指標表
/// 對應資料庫 metrics 表
/// </summary>
[Table("metrics")]
public class Metric
{
    /// <summary>
    /// 指標ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MetricID { get; set; }

    /// <summary>
    /// 所屬來源ID
    /// </summary>
    public int SourceID { get; set; }

    /// <summary>
    /// 指標代碼 (concurrent_users/forum_posts...)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 單位 (users/posts/views)
    /// </summary>
    [StringLength(20)]
    public string? Unit { get; set; }

    /// <summary>
    /// 指標中文說明
    /// </summary>
    [StringLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬來源
    /// </summary>
    public virtual MetricSource Source { get; set; } = null!;

    /// <summary>
    /// 每日指標值
    /// </summary>
    public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
} 