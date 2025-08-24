using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 每日熱度指數實體 - 計算結果
/// 對應資料表: popularity_index_daily
/// </summary>
[Table("popularity_index_daily")]
public class PopularityIndexDaily
{
    /// <summary>
    /// 流水號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// 遊戲編號 (外鍵至 Game)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    [Required]
    [Column("date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// 熱度指數 (加權計算)
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
    /// 計算方法版本
    /// </summary>
    [Column("calculation_version")]
    [StringLength(20)]
    public string? CalculationVersion { get; set; }

    /// <summary>
    /// 權重配置JSON
    /// </summary>
    [Column("weight_config_json")]
    public string? WeightConfigJson { get; set; }

    /// <summary>
    /// 排名 (當日)
    /// </summary>
    [Column("daily_rank")]
    public int? DailyRank { get; set; }

    /// <summary>
    /// 變化趨勢 (up/down/stable)
    /// </summary>
    [Column("trend")]
    [StringLength(10)]
    public string? Trend { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;
}