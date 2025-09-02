using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 論壇版面表
/// 對應資料庫 forums 表
/// </summary>
[Table("forums")]
public class Forum
{
    [Key]
    public int ForumID { get; set; }
    public int GameID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 