using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 論壇回覆表
/// 對應資料庫 thread_posts 表
/// </summary>
[Table("thread_posts")]
public class ThreadPost
{
    [Key]
    public long ID { get; set; }
    public long ThreadID { get; set; }
    public int AuthorUserID { get; set; }
    public string ContentMD { get; set; } = string.Empty;
    public long? ParentPostID { get; set; }
    public string Status { get; set; } = "normal";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 