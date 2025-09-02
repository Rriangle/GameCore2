using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 洞察貼文表
/// 對應資料庫 posts 表
/// </summary>
[Table("posts")]
public class Post
{
    [Key]
    public int PostID { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? GameID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TLDR { get; set; }
    public string BodyMD { get; set; } = string.Empty;
    public bool Visibility { get; set; } = true;
    public string Status { get; set; } = "draft";
    public bool Pinned { get; set; } = false;
    public int CreatedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 