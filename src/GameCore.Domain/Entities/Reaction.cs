using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通用讚表實體 - 多型反應系統
/// 對應資料表: reactions
/// </summary>
[Table("reactions")]
public class Reaction
{
    /// <summary>
    /// 流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 誰按的 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 目標類型 (post/thread/thread_post)
    /// </summary>
    [Required]
    [Column("target_type")]
    [StringLength(50)]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號 (多型，不設FK)
    /// </summary>
    [Required]
    [Column("target_id")]
    public long TargetId { get; set; }

    /// <summary>
    /// 反應類型 (like/love/haha/wow/sad/angry等)
    /// </summary>
    [Required]
    [Column("kind")]
    [StringLength(20)]
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 反應強度 (1-5，預設1)
    /// </summary>
    [Column("intensity")]
    public int Intensity { get; set; } = 1;

    /// <summary>
    /// 自定義表情符號 (Unicode或圖片URL)
    /// </summary>
    [Column("custom_emoji")]
    [StringLength(100)]
    public string? CustomEmoji { get; set; }

    /// <summary>
    /// 反應說明文字 (可選)
    /// </summary>
    [Column("comment")]
    [StringLength(200)]
    public string? Comment { get; set; }

    /// <summary>
    /// IP位址
    /// </summary>
    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 是否為匿名反應
    /// </summary>
    [Column("is_anonymous")]
    public bool IsAnonymous { get; set; } = false;

    /// <summary>
    /// 反應權重 (用於排序和統計)
    /// </summary>
    [Column("weight", TypeName = "decimal(3,2)")]
    public decimal Weight { get; set; } = 1.0m;

    /// <summary>
    /// 導航屬性 - 反應的用戶
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}