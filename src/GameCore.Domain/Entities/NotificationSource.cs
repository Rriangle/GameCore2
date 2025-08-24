using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通知來源類型實體 - 字典表
/// 對應資料表: Notification_Sources
/// </summary>
[Table("Notification_Sources")]
public class NotificationSource
{
    /// <summary>
    /// 來源類型編號 (主鍵)
    /// </summary>
    [Key]
    [Column("source_id")]
    public int SourceId { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    [Required]
    [Column("source_name")]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 來源描述
    /// </summary>
    [Column("source_description")]
    [StringLength(500)]
    public string? SourceDescription { get; set; }

    /// <summary>
    /// 來源圖標
    /// </summary>
    [Column("source_icon")]
    [StringLength(100)]
    public string? SourceIcon { get; set; }

    /// <summary>
    /// 來源顏色
    /// </summary>
    [Column("source_color")]
    [StringLength(7)]
    public string? SourceColor { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序順序
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 通知列表
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}