using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 論壇主題表
/// 對應資料庫 threads 表
/// </summary>
[Table("threads")]
public class ForumThread
{
    [Key]
    public long ThreadID { get; set; }
    public int ForumID { get; set; }
    public int AuthorUserID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "normal";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 