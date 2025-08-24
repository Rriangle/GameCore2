using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 洞察引用來源清單實體
/// 對應資料表: post_sources
/// </summary>
[Table("post_sources")]
public class PostSource
{
    /// <summary>
    /// 流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 文章編號 (外鍵至 Post)
    /// </summary>
    [Required]
    [Column("post_id")]
    public long PostId { get; set; }

    /// <summary>
    /// 顯示名稱
    /// </summary>
    [Required]
    [Column("source_name")]
    [StringLength(200)]
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 外部連結
    /// </summary>
    [Required]
    [Column("url")]
    [StringLength(1000)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 來源類型 (official/news/blog/social/research)
    /// </summary>
    [Column("source_type")]
    [StringLength(20)]
    public string? SourceType { get; set; }

    /// <summary>
    /// 來源描述
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 發佈日期 (來源的發佈時間)
    /// </summary>
    [Column("published_date")]
    public DateTime? PublishedDate { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [Column("author")]
    [StringLength(100)]
    public string? Author { get; set; }

    /// <summary>
    /// 是否已驗證
    /// </summary>
    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    /// <summary>
    /// 排序序號
    /// </summary>
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 導航屬性 - 所屬文章
    /// </summary>
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;
}