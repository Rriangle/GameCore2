using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 論壇版主檔實體 - 每遊戲一個版
/// 對應資料表: forums
/// </summary>
[Table("forums")]
public class Forum
{
    /// <summary>
    /// 論壇版編號 (主鍵)
    /// </summary>
    [Key]
    [Column("forum_id")]
    public int ForumId { get; set; }

    /// <summary>
    /// 遊戲編號 (外鍵至 Game，一對一)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 版名
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 版說明
    /// </summary>
    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 論壇狀態 (active/locked/archived)
    /// </summary>
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 最後活動時間
    /// </summary>
    [Column("last_activity_at")]
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// 主題總數
    /// </summary>
    [Column("thread_count")]
    public int ThreadCount { get; set; } = 0;

    /// <summary>
    /// 回覆總數
    /// </summary>
    [Column("post_count")]
    public int PostCount { get; set; } = 0;

    /// <summary>
    /// 論壇規則
    /// </summary>
    [Column("rules")]
    public string? Rules { get; set; }

    /// <summary>
    /// 是否允許遊客瀏覽
    /// </summary>
    [Column("allow_guest_view")]
    public bool AllowGuestView { get; set; } = true;

    /// <summary>
    /// 是否需要審核
    /// </summary>
    [Column("require_approval")]
    public bool RequireApproval { get; set; } = false;

    /// <summary>
    /// 導航屬性 - 關聯遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 論壇內的主題
    /// </summary>
    public virtual ICollection<Thread> Threads { get; set; } = new List<Thread>();
}