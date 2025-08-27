using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通知行為類型實體 - 字典表
/// 對應資料表: Notification_Actions
/// </summary>
[Table("Notification_Actions")]
public class NotificationAction
{
    /// <summary>
    /// 行為類型編號 (主鍵)
    /// </summary>
    [Key]
    [Column("action_id")]
    public int ActionId { get; set; }

    /// <summary>
    /// 行為名稱 (唯一)
    /// </summary>
    [Required]
    [Column("action_name")]
    [StringLength(100)]
    public string ActionName { get; set; } = string.Empty;

    /// <summary>
    /// 行為描述
    /// </summary>
    [Column("action_description")]
    [StringLength(500)]
    public string? ActionDescription { get; set; }

    /// <summary>
    /// 行為模板 (用於生成通知內容)
    /// </summary>
    [Column("action_template")]
    [StringLength(1000)]
    public string? ActionTemplate { get; set; }

    /// <summary>
    /// 行為圖標
    /// </summary>
    [Column("action_icon")]
    [StringLength(100)]
    public string? ActionIcon { get; set; }

    /// <summary>
    /// 行為顏色
    /// </summary>
    [Column("action_color")]
    [StringLength(7)]
    public string? ActionColor { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("is_enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 優先級 (數值越高優先級越高)
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;

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