using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 主題回覆實體 - 支援二層結構
/// 對應資料表: thread_posts
/// </summary>
[Table("thread_posts")]
public class ThreadPost
{
    /// <summary>
    /// 回覆編號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 主題編號 (外鍵至 Thread)
    /// </summary>
    [Required]
    [Column("thread_id")]
    public long ThreadId { get; set; }

    /// <summary>
    /// 回覆者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("author_user_id")]
    public int AuthorUserId { get; set; }

    /// <summary>
    /// 內容 (Markdown)
    /// </summary>
    [Required]
    [Column("content_md")]
    public string ContentMd { get; set; } = string.Empty;

    /// <summary>
    /// 父回覆編號 (支援二層，外鍵至 ThreadPost)
    /// </summary>
    [Column("parent_post_id")]
    public long? ParentPostId { get; set; }

    /// <summary>
    /// 狀態 (normal/hidden/deleted)
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
    /// 是否已編輯
    /// </summary>
    [Column("is_edited")]
    public bool IsEdited { get; set; } = false;

    /// <summary>
    /// 編輯時間
    /// </summary>
    [Column("edited_at")]
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// 編輯原因
    /// </summary>
    [Column("edit_reason")]
    [StringLength(200)]
    public string? EditReason { get; set; }

    /// <summary>
    /// IP位址
    /// </summary>
    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用戶代理
    /// </summary>
    [Column("user_agent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 點讚數
    /// </summary>
    [Column("like_count")]
    public int LikeCount { get; set; } = 0;

    /// <summary>
    /// 樓層號 (在主題中的序號)
    /// </summary>
    [Column("floor_number")]
    public int FloorNumber { get; set; } = 0;

    /// <summary>
    /// 附件檔案路徑 (JSON格式)
    /// </summary>
    [Column("attachments_json")]
    public string? AttachmentsJson { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬主題
    /// </summary>
    [ForeignKey("ThreadId")]
    public virtual Thread Thread { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 作者
    /// </summary>
    [ForeignKey("AuthorUserId")]
    public virtual User Author { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 父回覆
    /// </summary>
    [ForeignKey("ParentPostId")]
    public virtual ThreadPost? ParentPost { get; set; }

    /// <summary>
    /// 導航屬性 - 子回覆列表
    /// </summary>
    public virtual ICollection<ThreadPost> ChildPosts { get; set; } = new List<ThreadPost>();
}