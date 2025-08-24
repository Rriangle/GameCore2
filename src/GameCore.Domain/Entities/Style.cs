using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 樣式項目實體 - 字典表
/// 對應資料表: Styles
/// </summary>
[Table("Styles")]
public class Style
{
    /// <summary>
    /// 樣式編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("style_id")]
    public int StyleId { get; set; }

    /// <summary>
    /// 樣式名稱
    /// </summary>
    [Required]
    [Column("style_name")]
    [StringLength(100)]
    public string StyleName { get; set; } = string.Empty;

    /// <summary>
    /// 效果說明
    /// </summary>
    [Required]
    [Column("effect_desc")]
    [StringLength(500)]
    public string EffectDesc { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 設置者編號 (外鍵至 ManagerRole.Manager_Id)
    /// </summary>
    [Required]
    [Column("manager_id")]
    public int ManagerId { get; set; }

    /// <summary>
    /// 樣式類型 (theme/color/font/layout)
    /// </summary>
    [Column("StyleType")]
    [StringLength(20)]
    public string StyleType { get; set; } = "theme";

    /// <summary>
    /// CSS樣式內容
    /// </summary>
    [Column("CssContent")]
    public string? CssContent { get; set; }

    /// <summary>
    /// JavaScript內容
    /// </summary>
    [Column("JsContent")]
    public string? JsContent { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否為預設樣式
    /// </summary>
    [Column("IsDefault")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 適用範圍 (all/forum/store/admin)
    /// </summary>
    [Column("ApplicableScope")]
    [StringLength(20)]
    public string ApplicableScope { get; set; } = "all";

    /// <summary>
    /// 樣式版本
    /// </summary>
    [Column("StyleVersion")]
    [StringLength(20)]
    public string StyleVersion { get; set; } = "1.0.0";

    /// <summary>
    /// 相容性標籤
    /// </summary>
    [Column("CompatibilityTags")]
    [StringLength(200)]
    public string? CompatibilityTags { get; set; }

    /// <summary>
    /// 預覽圖片URL
    /// </summary>
    [Column("PreviewImage")]
    [StringLength(500)]
    public string? PreviewImage { get; set; }

    /// <summary>
    /// 樣式配置 (JSON格式)
    /// </summary>
    [Column("StyleConfig")]
    public string? StyleConfig { get; set; }

    /// <summary>
    /// 使用次數
    /// </summary>
    [Column("UsageCount")]
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// 最後使用時間
    /// </summary>
    [Column("LastUsedAt")]
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    [Column("SortOrder")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 樣式優先級
    /// </summary>
    [Column("Priority")]
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 樣式標籤 (JSON格式)
    /// </summary>
    [Column("StyleTags")]
    public string? StyleTags { get; set; }

    /// <summary>
    /// 是否需要重新載入
    /// </summary>
    [Column("RequiresReload")]
    public bool RequiresReload { get; set; } = false;

    /// <summary>
    /// 樣式描述
    /// </summary>
    [Column("StyleDescription")]
    [StringLength(1000)]
    public string? StyleDescription { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 導航屬性 - 設置者
    /// </summary>
    [ForeignKey("ManagerId")]
    public virtual ManagerData Manager { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 更新者
    /// </summary>
    [ForeignKey("UpdatedBy")]
    public virtual ManagerData? UpdatedByManager { get; set; }
}