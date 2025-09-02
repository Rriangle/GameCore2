using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 遊戲主檔表
/// 對應資料庫 games 表
/// </summary>
[Table("games")]
public class Game
{
    /// <summary>
    /// 遊戲ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GameID { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 類型 (FPS/MOBA...)
    /// </summary>
    [StringLength(50)]
    public string? Genre { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 論壇版面
    /// </summary>
    public virtual Forum? Forum { get; set; }

    /// <summary>
    /// 遊戲來源對應
    /// </summary>
    public virtual ICollection<GameSourceMap> GameSourceMaps { get; set; } = new List<GameSourceMap>();

    /// <summary>
    /// 每日指標
    /// </summary>
    public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();

    /// <summary>
    /// 熱度指數
    /// </summary>
    public virtual ICollection<PopularityIndexDaily> PopularityIndexDailies { get; set; } = new List<PopularityIndexDaily>();

    /// <summary>
    /// 排行榜快照
    /// </summary>
    public virtual ICollection<LeaderboardSnapshot> LeaderboardSnapshots { get; set; } = new List<LeaderboardSnapshot>();

    /// <summary>
    /// 洞察貼文
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
} 