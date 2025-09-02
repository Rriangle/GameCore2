using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 外部ID對應表
/// 對應資料庫 game_source_map 表
/// </summary>
[Table("game_source_map")]
public class GameSourceMap
{
    /// <summary>
    /// 對應ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    /// <summary>
    /// 內部遊戲ID
    /// </summary>
    public int GameID { get; set; }

    /// <summary>
    /// 外部來源ID
    /// </summary>
    public int SourceID { get; set; }

    /// <summary>
    /// 外部ID (Steam appid / 巴哈 slug)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ExternalKey { get; set; } = string.Empty;

    // 導航屬性
    /// <summary>
    /// 關聯的遊戲
    /// </summary>
    public virtual Game Game { get; set; } = null!;

    /// <summary>
    /// 關聯的來源
    /// </summary>
    public virtual MetricSource Source { get; set; } = null!;
} 