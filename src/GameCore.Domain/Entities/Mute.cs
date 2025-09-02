using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 禁言選項表
/// 對應資料庫 Mutes 表
/// </summary>
[Table("Mutes")]
public class Mute
{
    /// <summary>
    /// 禁言選項ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MuteID { get; set; }

    /// <summary>
    /// 禁言名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string MuteName { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 設置者ID
    /// </summary>
    public int ManagerID { get; set; }

    // 導航屬性
    /// <summary>
    /// 設置的管理者
    /// </summary>
    public virtual ManagerData Manager { get; set; } = null!;
} 