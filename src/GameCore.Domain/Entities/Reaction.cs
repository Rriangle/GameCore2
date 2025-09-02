using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 反應表
/// 對應資料庫 reactions 表
/// </summary>
[Table("reactions")]
public class Reaction
{
    [Key]
    public long ID { get; set; }
    public int UserID { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public long TargetID { get; set; }
    public string Kind { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 