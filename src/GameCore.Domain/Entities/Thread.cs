using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 版內主題實體 - 討論串
/// 對應資料表: threads
/// </summary>
[Table("threads")]
public class Thread
{
    /// <summary>
    /// 主題編號 (主鍵)
    /// </summary>
    [Key]
    [Column("thread_id")]
    public long ThreadId { get; set; }

    /// <summary>
    /// 所屬版編號 (外鍵至 Forum)
    /// </summary>
    [Required]
    [Column("forum_id")]
    public int ForumId { get; set; }

    /// <summary>
    /// 作者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("author_user_id")]
    public int AuthorUserId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    [Required]
    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (normal/hidden/archived)
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "normal";

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 是否置頂
    /// </summary>
    [Column("is_pinned")]
    public bool IsPinned { get; set; } = false;

    /// <summary>
    /// 是否鎖定 (不可回覆)
    /// </summary>
    [Column("is_locked")]
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// 瀏覽次數
    /// </summary>
    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 回覆數量
    /// </summary>
    [Column("reply_count")]
    public int ReplyCount { get; set; } = 0;

    /// <summary>
    /// 最後回覆時間
    /// </summary>
    [Column("last_reply_at")]
    public DateTime? LastReplyAt { get; set; }

    /// <summary>
    /// 最後回覆者編號
    /// </summary>
    [Column("last_reply_user_id")]
    public int? LastReplyUserId { get; set; }

    /// <summary>
    /// 標籤 (JSON格式)
    /// </summary>
    [Column("tags_json")]
    public string? TagsJson { get; set; }

    /// <summary>
    /// 主題內容 (首篇內容)
    /// </summary>
    [Column("content_md")]
    public string? ContentMd { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬論壇
    /// </summary>
    [ForeignKey("ForumId")]
    public virtual Forum Forum { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 作者
    /// </summary>
    [ForeignKey("AuthorUserId")]
    public virtual User Author { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 最後回覆者
    /// </summary>
    [ForeignKey("LastReplyUserId")]
    public virtual User? LastReplyUser { get; set; }

    /// <summary>
    /// 導航屬性 - 主題內的回覆
    /// </summary>
    public virtual ICollection<ThreadPost> Posts { get; set; } = new List<ThreadPost>();
}