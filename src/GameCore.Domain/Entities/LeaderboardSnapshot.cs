using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 榜單快照實體 - 直接給前台讀，避免重算
/// 對應資料表: leaderboard_snapshots
/// </summary>
[Table("leaderboard_snapshots")]
public class LeaderboardSnapshot
{
    /// <summary>
    /// 快照編號 (主鍵)
    /// </summary>
    [Key]
    [Column("snapshot_id")]
    public long SnapshotId { get; set; }

    /// <summary>
    /// 期間類型 (daily/weekly)
    /// </summary>
    [Required]
    [Column("period")]
    [StringLength(20)]
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 快照時間
    /// </summary>
    [Required]
    [Column("ts")]
    public DateTime Ts { get; set; }

    /// <summary>
    /// 名次 (1..N)
    /// </summary>
    [Required]
    [Column("rank")]
    public int Rank { get; set; }

    /// <summary>
    /// 遊戲編號 (外鍵至 Game)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 當時指數值
    /// </summary>
    [Required]
    [Column("index_value", TypeName = "decimal(18,4)")]
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 排名變化 (與上期比較)
    /// </summary>
    [Column("rank_change")]
    public int? RankChange { get; set; }

    /// <summary>
    /// 指數變化百分比
    /// </summary>
    [Column("index_change_percentage", TypeName = "decimal(8,4)")]
    public decimal? IndexChangePercentage { get; set; }

    /// <summary>
    /// 快照類型 (real_time/scheduled/manual)
    /// </summary>
    [Column("snapshot_type")]
    [StringLength(20)]
    public string? SnapshotType { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("note")]
    [StringLength(200)]
    public string? Note { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;
}