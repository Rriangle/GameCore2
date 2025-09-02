using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 排行榜快照表
/// 對應資料庫 leaderboard_snapshots 表
/// </summary>
[Table("leaderboard_snapshots")]
public class LeaderboardSnapshot
{
    /// <summary>
    /// 快照ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long SnapshotID { get; set; }

    /// <summary>
    /// 期間類型 (daily/weekly)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 快照時間
    /// </summary>
    public DateTime TS { get; set; }

    /// <summary>
    /// 名次 (1..N)
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 遊戲ID
    /// </summary>
    public int GameID { get; set; }

    /// <summary>
    /// 當時指數值
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的遊戲
    /// </summary>
    public virtual Game Game { get; set; } = null!;
} 