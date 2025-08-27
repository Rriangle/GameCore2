using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 統一貼文表實體 - 洞察與未來UGC都走這
/// 對應資料表: posts
/// </summary>
[Table("posts")]
public class Post
{
    /// <summary>
    /// 文章編號 (主鍵)
    /// </summary>
    [Key]
    [Column("post_id")]
    public int PostId { get; set; }

    /// <summary>
    /// 類型 (insight/user)
    /// </summary>
    [Required]
    [Column("type")]
    [StringLength(20)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 關聯遊戲編號 (可為NULL)
    /// </summary>
    [Column("game_id")]
    public int? GameId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    [Required]
    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 三行摘要 (卡片用)
    /// </summary>
    [Column("tldr")]
    [StringLength(500)]
    public string? Tldr { get; set; }

    /// <summary>
    /// 內文 (Markdown)
    /// </summary>
    [Required]
    [Column("body_md")]
    public string BodyMd { get; set; } = string.Empty;

    /// <summary>
    /// 可見性 (public/hidden)
    /// </summary>
    [Required]
    [Column("visibility")]
    public bool Visibility { get; set; } = true;

    /// <summary>
    /// 狀態 (draft/published/hidden)
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "draft";

    /// <summary>
    /// 是否置頂
    /// </summary>
    [Required]
    [Column("pinned")]
    public bool Pinned { get; set; } = false;

    /// <summary>
    /// 作者編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("created_by")]
    public int CreatedBy { get; set; }

    /// <summary>
    /// 發佈時間
    /// </summary>
    [Column("published_at")]
    public DateTime? PublishedAt { get; set; }

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
    /// 瀏覽次數
    /// </summary>
    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 點讚數
    /// </summary>
    [Column("like_count")]
    public int LikeCount { get; set; } = 0;

    /// <summary>
    /// 封面圖片URL
    /// </summary>
    [Column("cover_image_url")]
    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    /// <summary>
    /// 標籤 (JSON格式)
    /// </summary>
    [Column("tags_json")]
    public string? TagsJson { get; set; }

    /// <summary>
    /// 導航屬性 - 關聯遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game? Game { get; set; }

    /// <summary>
    /// 導航屬性 - 作者
    /// </summary>
    [ForeignKey("CreatedBy")]
    public virtual User Author { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 文章的數據快照
    /// </summary>
    public virtual PostMetricSnapshot? MetricSnapshot { get; set; }

    /// <summary>
    /// 導航屬性 - 文章的引用來源
    /// </summary>
    public virtual ICollection<PostSource> Sources { get; set; } = new List<PostSource>();
}