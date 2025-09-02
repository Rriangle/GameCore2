using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 樣式池表
/// 對應資料庫 Styles 表
/// </summary>
[Table("Styles")]
public class Style
{
    /// <summary>
    /// 樣式ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StyleID { get; set; }

    /// <summary>
    /// 樣式名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string StyleName { get; set; } = string.Empty;

    /// <summary>
    /// 效果說明
    /// </summary>
    [StringLength(500)]
    public string? EffectDesc { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

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