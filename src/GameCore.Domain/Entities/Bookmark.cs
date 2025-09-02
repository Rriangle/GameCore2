using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 收藏表
/// 對應資料庫 bookmarks 表
/// </summary>
[Table("bookmarks")]
public class Bookmark
{
    [Key]
    public long ID { get; set; }
    public int UserID { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public long TargetID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 