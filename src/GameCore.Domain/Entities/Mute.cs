using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 禁言項目實體 - 字典表
/// 對應資料表: Mutes
/// </summary>
[Table("Mutes")]
public class Mute
{
    /// <summary>
    /// 禁言選項編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("mute_id")]
    public int MuteId { get; set; }

    /// <summary>
    /// 禁言名稱
    /// </summary>
    [Required]
    [Column("mute_name")]
    [StringLength(100)]
    public string MuteName { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 設置者編號 (外鍵至 ManagerRole.Manager_Id)
    /// </summary>
    [Required]
    [Column("manager_id")]
    public int ManagerId { get; set; }

    /// <summary>
    /// 禁言描述
    /// </summary>
    [Column("MuteDescription")]
    [StringLength(500)]
    public string? MuteDescription { get; set; }

    /// <summary>
    /// 禁言類型 (temporary/permanent/warning)
    /// </summary>
    [Column("MuteType")]
    [StringLength(20)]
    public string MuteType { get; set; } = "temporary";

    /// <summary>
    /// 預設禁言時長 (分鐘)
    /// </summary>
    [Column("DefaultDurationMinutes")]
    public int? DefaultDurationMinutes { get; set; }

    /// <summary>
    /// 嚴重程度 (1-5，數值越高越嚴重)
    /// </summary>
    [Column("SeverityLevel")]
    public int SeverityLevel { get; set; } = 1;

    /// <summary>
    /// 適用範圍 (forum/chat/all)
    /// </summary>
    [Column("ApplicableScope")]
    [StringLength(20)]
    public string ApplicableScope { get; set; } = "all";

    /// <summary>
    /// 自動解除 (是否自動解除禁言)
    /// </summary>
    [Column("AutoRelease")]
    public bool AutoRelease { get; set; } = true;

    /// <summary>
    /// 警告訊息模板
    /// </summary>
    [Column("WarningTemplate")]
    [StringLength(1000)]
    public string? WarningTemplate { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    [Column("SortOrder")]
    public int SortOrder { get; set; } = 0;

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
    /// 禁言顏色 (用於UI顯示)
    /// </summary>
    [Column("MuteColor")]
    [StringLength(7)]
    public string? MuteColor { get; set; }

    /// <summary>
    /// 禁言圖標
    /// </summary>
    [Column("MuteIcon")]
    [StringLength(100)]
    public string? MuteIcon { get; set; }

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