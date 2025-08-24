using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 遊戲主檔實體 - 管理平台所有遊戲清單
/// 對應資料表: games
/// </summary>
[Table("games")]
public class Game
{
    /// <summary>
    /// 遊戲編號 (主鍵)
    /// </summary>
    [Key]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 遊戲類型 (FPS/MOBA/RPG等)
    /// </summary>
    [Required]
    [Column("genre")]
    [StringLength(100)]
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 遊戲描述
    /// </summary>
    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 遊戲圖片URL
    /// </summary>
    [Column("image_url")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 遊戲狀態 (active/inactive)
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 導航屬性 - 遊戲的外部來源對應
    /// </summary>
    public virtual ICollection<GameSourceMap> SourceMaps { get; set; } = new List<GameSourceMap>();

    /// <summary>
    /// 導航屬性 - 遊戲的每日指標數據
    /// </summary>
    public virtual ICollection<GameMetricDaily> MetricData { get; set; } = new List<GameMetricDaily>();

    /// <summary>
    /// 導航屬性 - 遊戲的每日熱度指數
    /// </summary>
    public virtual ICollection<PopularityIndexDaily> PopularityIndexes { get; set; } = new List<PopularityIndexDaily>();

    /// <summary>
    /// 導航屬性 - 遊戲的排行榜快照
    /// </summary>
    public virtual ICollection<LeaderboardSnapshot> LeaderboardSnapshots { get; set; } = new List<LeaderboardSnapshot>();

    /// <summary>
    /// 導航屬性 - 與遊戲相關的洞察文章
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}