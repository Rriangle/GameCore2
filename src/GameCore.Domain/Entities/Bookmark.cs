using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通用收藏表實體 - 多型收藏系統
/// 對應資料表: bookmarks
/// </summary>
[Table("bookmarks")]
public class Bookmark
{
    /// <summary>
    /// 流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 收藏者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 目標類型 (post/thread/game/forum)
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
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 收藏分類 (default/favorites/read_later等)
    /// </summary>
    [Column("category")]
    [StringLength(50)]
    public string Category { get; set; } = "default";

    /// <summary>
    /// 收藏標籤 (JSON格式)
    /// </summary>
    [Column("tags_json")]
    public string? TagsJson { get; set; }

    /// <summary>
    /// 個人備註
    /// </summary>
    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 收藏時的標題快照
    /// </summary>
    [Column("title_snapshot")]
    [StringLength(200)]
    public string? TitleSnapshot { get; set; }

    /// <summary>
    /// 收藏時的內容摘要快照
    /// </summary>
    [Column("content_snippet")]
    [StringLength(500)]
    public string? ContentSnippet { get; set; }

    /// <summary>
    /// 是否為私人收藏
    /// </summary>
    [Column("is_private")]
    public bool IsPrivate { get; set; } = false;

    /// <summary>
    /// 排序順序
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 最後訪問時間
    /// </summary>
    [Column("last_accessed_at")]
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// 訪問次數
    /// </summary>
    [Column("access_count")]
    public int AccessCount { get; set; } = 0;

    /// <summary>
    /// 收藏來源 (manual/auto/import)
    /// </summary>
    [Column("source")]
    [StringLength(20)]
    public string Source { get; set; } = "manual";

    /// <summary>
    /// 導航屬性 - 收藏的用戶
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}