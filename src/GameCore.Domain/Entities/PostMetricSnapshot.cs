using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 洞察發佈時的數據快照實體 - 固定展示
/// 對應資料表: post_metric_snapshot
/// </summary>
[Table("post_metric_snapshot")]
public class PostMetricSnapshot
{
    /// <summary>
    /// 文章編號 (主鍵，外鍵至 Post)
    /// </summary>
    [Key]
    [Column("post_id")]
    public long PostId { get; set; }

    /// <summary>
    /// 當時的遊戲編號 (外鍵至 Game)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 拍照日期
    /// </summary>
    [Required]
    [Column("date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// 當日指數
    /// </summary>
    [Required]
    [Column("index_value", TypeName = "decimal(18,4)")]
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 當日各指標值/Δ%/權重 (JSON)
    /// </summary>
    [Required]
    [Column("details_json")]
    public string DetailsJson { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 當日排名
    /// </summary>
    [Column("daily_rank")]
    public int? DailyRank { get; set; }

    /// <summary>
    /// 指數變化 (與前日比較)
    /// </summary>
    [Column("index_change", TypeName = "decimal(18,4)")]
    public decimal? IndexChange { get; set; }

    /// <summary>
    /// 指數變化百分比
    /// </summary>
    [Column("index_change_percentage", TypeName = "decimal(8,4)")]
    public decimal? IndexChangePercentage { get; set; }

    /// <summary>
    /// 快照版本
    /// </summary>
    [Column("snapshot_version")]
    [StringLength(20)]
    public string? SnapshotVersion { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬文章
    /// </summary>
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 關聯遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;
}